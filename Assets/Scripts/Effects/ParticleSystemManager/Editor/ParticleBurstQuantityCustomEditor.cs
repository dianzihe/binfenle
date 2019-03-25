using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ParticleBurstQuantity))]
	
public class ParticleBurstQuantityCustomEditor : Editor { 

	public override void OnInspectorGUI()
	{
		//target.lookAtPoint = EditorGUILayout.Vector3Field ("Look At Point", target.lookAtPoint);
		
		EditorGUIUtility.LookLikeInspector();
		DrawDefaultInspector();
		
		ParticleBurstQuantity burstScript = target as ParticleBurstQuantity; 
		
		
		if (GUILayout.Button("Sync burst data"))
		{
			foreach(ParticleSystem iteratorParticleSystem in burstScript.GetComponentsInChildren<ParticleSystem>())
			{
				ParticleBurstQuantity tempParticleBurstQuantity = iteratorParticleSystem.GetComponent<ParticleBurstQuantity>();
				
				if (tempParticleBurstQuantity == null)
					tempParticleBurstQuantity = iteratorParticleSystem.gameObject.AddComponent<ParticleBurstQuantity>();
				
				SerializedObject serializedObject = new SerializedObject(iteratorParticleSystem);
				
				SerializedProperty serializedBurst 		= serializedObject.FindProperty("EmissionModule.cnt0");
			
				tempParticleBurstQuantity.burstQuantity = serializedBurst.intValue;
			}
		}
		
		if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}
