using UnityEngine;
using static UnityEngine.ParticleSystem;
#if UNITY_EDITOR
using UnityEditor;


public class csShurikenEffectChanger : MonoBehaviour
{

	private ParticleSystem.MainModule _mainModule;

	public void ShurikenParticleScaleChange(float _Value)
	{
		ParticleSystem[] ParticleSystems = GetComponentsInChildren<ParticleSystem>();

        transform.localScale *= _Value;

		foreach(ParticleSystem _ParticleSystem in ParticleSystems) {

			_mainModule = _ParticleSystem.main;
			_mainModule.startSpeedMultiplier *= _Value;
			_mainModule.startSizeMultiplier *= _Value;
			_mainModule.gravityModifierMultiplier *= _Value;
			SerializedObject _SerializedObject = new SerializedObject(_ParticleSystem);
			_SerializedObject.FindProperty("CollisionModule.particleRadius").floatValue *= _Value;
			_SerializedObject.FindProperty("ShapeModule.radius").floatValue *= _Value;
			_SerializedObject.FindProperty("ShapeModule.boxX").floatValue *= _Value;
			_SerializedObject.FindProperty("ShapeModule.boxY").floatValue *= _Value;
			_SerializedObject.FindProperty("ShapeModule.boxZ").floatValue *= _Value;
			_SerializedObject.FindProperty("VelocityModule.x.scalar").floatValue *= _Value;
			_SerializedObject.FindProperty("VelocityModule.y.scalar").floatValue *= _Value;
			_SerializedObject.FindProperty("VelocityModule.z.scalar").floatValue *= _Value;
			_SerializedObject.FindProperty("ClampVelocityModule.x.scalar").floatValue *= _Value;
			_SerializedObject.FindProperty("ClampVelocityModule.y.scalar").floatValue *= _Value;
			_SerializedObject.FindProperty("ClampVelocityModule.z.scalar").floatValue *= _Value;
			_SerializedObject.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= _Value;
			_SerializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
