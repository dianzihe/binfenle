using UnityEngine;
using System.Collections;

public class BoardHolesMaskGenerator
{
	private int COLUMNS  = 8;
	private int ROWS 	 = 8;
	
	// Grid empty flags matrix
	private bool[,] gridEmptyCells =
	{
		{ false,	false,	false,	false,	false,	false,	false,	false },
		{ false,	false, 	false,  true,	true,	false,	true,	false },
		{ true,		true,	true,   true,	false,	false,	false,	false },
		{ true,		false,	true,   true,	false,	false,	true,	false },
		{ true,		true,	true,   true,	true,	false,	true,	false },
		{ false,	false,	false,  true,	true,	false,	true,	false },
		{ false,	false,	false,  false,	false,	false,	false,	false },
		{ false,	false,	false,  false,	false,	false,	false,	false },
	};

	//-----------------------------------------------------
	// PATTERNS: Textures
	//-----------------------------------------------------
	public Texture2D		patternA;
	public Texture2D		patternB;
	
	public Texture2D		mask;
	
	//-----------------------------------------------------
	// BOARD
	// UV coordinates of Start and End grid board inside the board texture
	//-----------------------------------------------------
	public Vector2 	uvTextureGridStart = new Vector2(0.135f, 0.1362f);
	public Vector2 	uvTextureSquaresEnd	  = new Vector2(0.88f, 0.89f);
	
	// Compute UV size of each grid cell
	private Vector2 squareSize;
	private Vector2 squareSizeSoftH;
	private Vector2 squareSizeSoftV;
	protected float softBorderFactor;
	
	protected BoardData boardData;
	
	
	public BoardHolesMaskGenerator(BoardData _boardData) 
	{
		boardData = _boardData;
		
		if (boardData == null) {
			Debug.LogError("[BoardHolesMaskGenerator] boardData is NULL! Wrong init order?!");
		}
		
	}
	
	public static Color ColorZero
	{
		get {
			return new Color(0f, 0f, 0f, 0f);
		}
	}
	
	public static Color ColorValue(float colorValue)
	{
		return new Color(colorValue, colorValue, colorValue, colorValue);
	}
	
	/// <summary>
	/// Creates the mask texture. You are responsible for destroying this texture when you're done with it.
	/// </summary>
	/// <returns>
	/// The mask texture.
	/// </returns>
	/// <param name='texWidth'>
	/// Tex width.
	/// </param>
	/// <param name='texHeight'>
	/// Tex height.
	/// </param>
	/// <param name='_softBorderFactor'>
	/// _soft border factor.
	/// </param>
	/// <param name='texFormat'>
	/// Tex format.
	/// </param>
	public Texture2D CreateMaskTexture(int texWidth, int texHeight, float _softBorderFactor = 0.1f, TextureFormat texFormat = TextureFormat.Alpha8, bool useTestGrid = false) 
	{
		softBorderFactor = _softBorderFactor;

		Texture2D alphaMask = new Texture2D(texWidth, texHeight, texFormat, false);
		alphaMask.anisoLevel = 0;
		alphaMask.wrapMode = TextureWrapMode.Clamp;
		
		if ( !useTestGrid ) {
			ReadGridCellsInfoFromBoard();
		}

		BakeMaskSoft(alphaMask);

		return alphaMask;
	}

	private void ComputeGridCellSize()
	{
		squareSize = uvTextureSquaresEnd - uvTextureGridStart;
		squareSize.x /= COLUMNS;
		squareSize.y /= ROWS;
		
		squareSizeSoftH = squareSize;
		squareSizeSoftH.x *= softBorderFactor;
		
		squareSizeSoftV = squareSize;
		squareSizeSoftV.y *= softBorderFactor;
	}
	
	void Fill(Texture2D texture, Vector2 startUV, Vector2 sizeUV, Color color)
	{
		int x0 = (int)(startUV.x * texture.width);
		int y0 = (int)(startUV.y * texture.height);
		
		int x1 = x0 + (int)(sizeUV.x * texture.width);
		int y1 = y0 + (int)(sizeUV.y * texture.height);

		for (int y = y0; y<= y1; y++) {
			for (int x = x0; x <= x1; x++) {
				texture.SetPixel(x, y, color);
			}
		}
	}
	
	
	void FillSmoothFromToH(Texture2D texture, Vector2 startUV, Vector2 sizeUV, Color colorA, Color colorB)
	{			
		int x0 = (int)(startUV.x * texture.width);
		int y0 = (int)(startUV.y * texture.height);		
		
		int x1 = x0 + (int)(sizeUV.x * texture.width);
		int y1 = y0 + (int)(sizeUV.y * texture.height);
		
		for (int y = y0; y <= y1; y++) {
			for (int x = x0; x <= x1; x++) 
			{
				float alphaOld = texture.GetPixel(x,y).a;
				float alphaNew = Color.Lerp(colorA, colorB, (x - x0) / (float)(x1 - x0)).a;
				
//				texture.SetPixel(x, y, Color.Lerp(colorA, colorB, (x - x0) / (float)(x1 - x0)) );
				texture.SetPixel(x, y, ColorValue(Mathf.Max(alphaOld, alphaNew)));
			}
		}
	}
		
	void FillSmoothFromToV(Texture2D texture, Vector2 startUV, Vector2 sizeUV, Color colorA, Color colorB)
	{
		int x0 = (int)(startUV.x * texture.width);
		int y0 = (int)(startUV.y * texture.height);
		
		int x1 = x0 + (int)(sizeUV.x * texture.width);
		int y1 = y0 + (int)(sizeUV.y * texture.height);
		
		for (int y = y0; y<= y1; y++) {
			for (int x = x0; x <= x1; x++)
			{
				float alphaOld = texture.GetPixel(x,y).a;
				float alphaNew = Color.Lerp(colorA, colorB, (y - y0) / (float)(y1 - y0)).a;

//				texture.SetPixel(x, y, Color.Lerp(colorA, colorB, (y - y0) / (float)(y1 - y0)) );
				texture.SetPixel(x, y, ColorValue(Mathf.Max(alphaOld, alphaNew)));
			}
		}
	}	
	
	
	void FillSmoothCircle(Texture2D texture, Vector2 circleCenterUV, float radiusUV, Color colorA, Color colorB)
	{
		Vector2 center = new Vector2(circleCenterUV.x * texture.width, circleCenterUV.y * texture.height);
		float radius = radiusUV * texture.width; // assuming the texture will always be square
		
		int x0 = (int)(center.x - radius);
		int y0 = (int)(center.y - radius);
		
		int x1 = (int)(center.x + radius);
		int y1 = (int)(center.y + radius);
		
		for(int x = x0; x <= x1; x++) {
			for(int y = y0; y <= y1; y++) 
			{
				float centerDistance = (new Vector2(x, y) - center).magnitude;
				if ( centerDistance <= radius ) {
					// Draw circle point
					float alphaOld = texture.GetPixel(x,y).a;
					float alphaNew = Color.Lerp(colorA, colorB, centerDistance / radius).a;
					
					// Draw the circle points only where there is a black area (where empty corners will be)
					if (alphaOld == 0f) {
						texture.SetPixel(x, y, ColorValue(alphaNew));
					}
//					texture.SetPixel(x, y, ColorValue(alphaNew));
				}
			}
		}
		
	}

	void ReadGridCellsInfoFromBoard()
	{
		for(int row = 0; row < boardData.NumRows; row++) {
			for(int col = 0; col < boardData.NumColumns; col++) {
				AbstractBoardPiece piece =  boardData[row, col];
				gridEmptyCells[row, col] = boardData[row, col].IsEmpty && piece.GetType() != typeof(GrowingThornBoardPiece) ;
			}
		}
	}
	
	private void DrawGradientBars(Texture2D mask, int row, int col)
	{
		// Right cell gradient bar
		if ( col == COLUMNS - 1 || !gridEmptyCells[row, col + 1] )
		{
			FillSmoothFromToH(mask, uvTextureGridStart + new Vector2((col + 1) * squareSize.x - squareSizeSoftH.x, (ROWS - 1 - row) * squareSize.y), 
							  squareSizeSoftH, ColorZero, Color.white);
		}
		
		// Left cell gradient bar
		if ( col == 0 || !gridEmptyCells[row, col - 1] )
		{
			FillSmoothFromToH(mask, uvTextureGridStart + new Vector2(col * squareSize.x, (ROWS - 1 - row) * squareSize.y),
						squareSizeSoftH, Color.white, ColorZero);
		}
		
		// Bottom cell gradient bar
		if ( row == ROWS - 1 || !gridEmptyCells[row + 1, col] )
		{
			FillSmoothFromToV(mask, uvTextureGridStart + new Vector2(col * squareSize.x, (ROWS - 1 - row) * squareSize.y), 
						squareSizeSoftV, Color.white, ColorZero);
		}

		// Top cell gradient bar
		if ( row == 0 || !gridEmptyCells[row - 1, col] )
		{
			FillSmoothFromToV(mask, uvTextureGridStart + new Vector2(col * squareSize.x, (ROWS - 1 - row + 1)  * squareSize.y - squareSizeSoftV.y),
							  squareSizeSoftV, ColorZero, Color.white);
		}	
	}
	
	private void DrawRoundedCorners(Texture2D mask, int row, int col)
	{				
		if (col > 0 && row > 0)
		{
			// Check Top-Left corner if we need to round it
			if ( gridEmptyCells[row - 1, col - 1] && gridEmptyCells[row, col - 1] && gridEmptyCells[row - 1, col] )
			{
				FillSmoothCircle(mask, uvTextureGridStart + new Vector2(col * squareSize.x, (ROWS - 1 - row + 1)  * squareSize.y),
								 squareSizeSoftH.x, Color.white, ColorZero);
			}
		}
		
		if (col > 0 && row + 1 < ROWS)
		{
			// Check Bottom-Left corner if we need to round it
			if ( gridEmptyCells[row + 1, col - 1] && gridEmptyCells[row, col - 1] && gridEmptyCells[row + 1, col] )
			{
				FillSmoothCircle(mask, uvTextureGridStart + new Vector2(col * squareSize.x, (ROWS - 1 - row)  * squareSize.y),
								 squareSizeSoftH.x, Color.white, ColorZero);
			}
		}
		
		if (col + 1 < COLUMNS && row > 0) 
		{
			// Check Top-Right corner if we need to round it
			if ( gridEmptyCells[row - 1, col + 1] && gridEmptyCells[row, col + 1] && gridEmptyCells[row - 1, col] )
			{
				FillSmoothCircle(mask, uvTextureGridStart + new Vector2(col * squareSize.x + squareSizeSoftV.x, (ROWS - 1 - row + 1)  * squareSize.y),
								 squareSizeSoftH.x, Color.white, ColorZero);
			}
		}
		
		if (col + 1 < COLUMNS && row + 1 < ROWS)
		{
			// Check Bottom-Right corner if we need to round it
			if ( gridEmptyCells[row + 1, col + 1] && gridEmptyCells[row, col + 1] && gridEmptyCells[row + 1, col] )
			{
				FillSmoothCircle(mask, uvTextureGridStart + new Vector2(col * squareSize.x + squareSizeSoftV.x, (ROWS - 1 - row)  * squareSize.y),
								 squareSizeSoftH.x, Color.white, ColorZero);
			}
		}
	}

	//-----------------------------------------------------
	// RASTER SOFT MASK
	//-----------------------------------------------------
	// _squares: empty data
	// _mask: alphaMask
	// _patternTrue: pattern to use when true
	// _patternFalse: pattern to use when false
	//-----------------------------------------------------
	public void BakeMaskSoft(Texture2D mask)
	{
		ComputeGridCellSize();
		
		// Reset mask texture
		Fill(mask, Vector2.zero, Vector2.one, Color.white);
		
		// Draw empty cells without gradient bars or rounded corners
		for (int row = 0; row < ROWS; row++) {
			for (int col = 0; col < COLUMNS; col++)
			{
				if (gridEmptyCells[row,col]) {
					Fill(mask, uvTextureGridStart + new Vector2(col * squareSize.x, (ROWS - 1 - row) * squareSize.y), squareSize, ColorZero);
				}
			}
		}

		// Draw empty cells gradient bars
		for (int row = 0; row < ROWS; row++) {
			for (int col = 0; col < COLUMNS; col++)
			{
				if (gridEmptyCells[row,col]) {
					DrawGradientBars(mask, row, col);
				}
			}
		}
		
		// Draw empty cells without gradient bars but only with rounded corners
		for (int row = 0; row < ROWS; row++) {
			for (int col = 0; col < COLUMNS; col++)
			{
				if ( !gridEmptyCells[row,col] ) {
					DrawRoundedCorners(mask, row, col);
				}
			}
		}

		mask.Apply();
	}
}
