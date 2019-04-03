

public interface ISelectableItem {
	event System.Action<ISelectableItem> SelectionEvent;
	event System.Action<ISelectableItem> EndSelectionEvent;
	string SelectableItemName ();
	void EnableItem(bool enable);

}
