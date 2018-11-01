using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class PIDI_2DReflection : MonoBehaviour {

	[HideInInspector]
	public Camera tempCamera;

	[HideInInspector]
	private Camera secondCam;

	[HideInInspector]
	public RenderTexture rt;
	[HideInInspector]
	public RenderTexture mask;

	[Range(1,5)]
	public int downScaleValue = 1;

	public float surfaceLevel = -99;
	public Color refColor = Color.white;
	public Color backgroundColor = Color.white;
	public LayerMask renderLayers;
	public LayerMask drawOverLayers;
	public bool alphaBackground;
	public bool srpMode;

	public Camera backgroundCam;
	public Camera foregroundCam;

	public bool isLocalReflection;
	public float waterSurfaceLine;
	public Vector2 horizontalLimits;
	public Transform reflectAsLocal;
	private Vector3 targetPos;


	private void OnEnable(){
		#if UNITY_2018_1_OR_NEWER && UNITY_EDITOR
		UnityEditor.EditorApplication.update += LateUpdate;
		#endif
	}

	void Start(){
		targetPos = transform.localPosition;
	}

	void OnDrawGizmosSelected()
	{
		if ( isLocalReflection ){
			Gizmos.color = Color.blue;
			Gizmos.DrawLine( new Vector3(transform.position.x-100, waterSurfaceLine, transform.position.z), new Vector3(transform.position.x+100,waterSurfaceLine,transform.position.z) );
			Gizmos.color = Color.red;
			Gizmos.DrawLine( new Vector3(horizontalLimits.x, transform.position.y+100, transform.position.z ), new Vector3( horizontalLimits.x, transform.position.y-100, transform.position.z ) );
			Gizmos.DrawLine( new Vector3(horizontalLimits.y, transform.position.y+100, transform.position.z ), new Vector3( horizontalLimits.y, transform.position.y-100, transform.position.z ) );
			Gizmos.color = Color.white;
		}	
	}

	void LateUpdate(){
		
		if ( Application.isPlaying&&isLocalReflection && reflectAsLocal ){
			transform.localPosition = targetPos;
			transform.position = new Vector3(Mathf.Clamp(reflectAsLocal.position.x, horizontalLimits.x, horizontalLimits.y), Mathf.Clamp(transform.position.y, -Mathf.Infinity, waterSurfaceLine), reflectAsLocal.position.z );	
		}

		#if UNITY_2018_1_OR_NEWER && UNITY_EDITOR
		if ( !Application.isPlaying && srpMode ){
			if ( UnityEditor.SceneView.lastActiveSceneView ){
				var c = UnityEditor.SceneView.lastActiveSceneView.camera;
				if ( c && c.cameraType == CameraType.SceneView ){
					SRPUpdate(c);
				}
			}
		}
		#endif
		

	}

	void OnWillRenderObject () {

		if ( srpMode ){
			return;
		}

		if ( !tempCamera ){
			tempCamera = new GameObject("TempPIDI2D_Camera", typeof(Camera) ).GetComponent<Camera>();
			tempCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
			tempCamera.enabled = false;
		}

		var downScale = 1.0f/downScaleValue;

		var myCam = Camera.current;

		if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection") ){
			myCam = foregroundCam;
		}

		if ( myCam.name == "TempPIDI2D_Camera" ){
			return;
		}
		
		if ( !rt ){
			rt = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}
		else if ( (int)(myCam.pixelWidth*downScale) != rt.width || (int)(myCam.pixelHeight*downScale) != rt.height ){
			DestroyImmediate(rt);
			rt = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}

		if ( !mask ){
			mask = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}
		else if ( (int)(myCam.pixelWidth*downScale) != rt.width || (int)(myCam.pixelHeight*downScale) != rt.height ){
			DestroyImmediate(rt);
			mask = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}

		

		if ( myCam ){
			tempCamera.transform.position = myCam.transform.position;
			tempCamera.transform.rotation = myCam.transform.rotation;
			tempCamera.orthographic = myCam.orthographic;
			tempCamera.orthographicSize = myCam.orthographicSize;
			tempCamera.fieldOfView = myCam.fieldOfView;
			tempCamera.aspect = myCam.aspect;
			tempCamera.cullingMask = renderLayers & ~(1<<4);
			tempCamera.targetTexture = rt;
			tempCamera.clearFlags = myCam.clearFlags==CameraClearFlags.Nothing?CameraClearFlags.SolidColor:myCam.clearFlags;
			tempCamera.backgroundColor = myCam.backgroundColor;
			tempCamera.backgroundColor = alphaBackground||GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection")?Color.clear:myCam.backgroundColor;
			#if UNITY_5_6_OR_NEWER
			tempCamera.allowHDR = myCam.allowHDR;
			tempCamera.allowMSAA = myCam.allowMSAA;
			#else
			tempCamera.hdr = myCam.hdr;
			#endif

			if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_Reflection2D") ){
				tempCamera.Render();

				MaterialPropertyBlock m = new MaterialPropertyBlock();
				GetComponent<Renderer>().GetPropertyBlock( m );
				if ( surfaceLevel == -99 ){
					surfaceLevel = GetComponent<Renderer>().sharedMaterial.GetFloat("_SurfaceLevel");
				}

				if ( refColor == Color.clear ){
					refColor = GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
				}

				if ( backgroundColor == Color.clear && GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection") ){
					backgroundColor = GetComponent<Renderer>().sharedMaterial.GetColor("_ColorB");
				}

				m.SetTexture( "_Reflection2D", rt );

				if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_ReflectionMask") ){
					tempCamera.clearFlags = CameraClearFlags.SolidColor;
					tempCamera.backgroundColor = Color.clear;
					tempCamera.cullingMask = drawOverLayers;
					tempCamera.targetTexture = mask;
					tempCamera.transform.position = myCam.transform.position;
					tempCamera.Render();
					m.SetTexture("_ReflectionMask",mask);
				}
				//GetComponent<Renderer>().sharedMaterial.SetTexture( "_Reflection2D", rt );
				m.SetColor("_Color",refColor);
				if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection") ){
					m.SetColor("_ColorB",backgroundColor);
					myCam = backgroundCam;
					if ( myCam ){
						tempCamera.transform.position = myCam.transform.position;
						tempCamera.transform.rotation = myCam.transform.rotation;
						tempCamera.orthographic = myCam.orthographic;
						tempCamera.orthographicSize = myCam.orthographicSize;
						tempCamera.fieldOfView = myCam.fieldOfView;
						tempCamera.aspect = myCam.aspect;
						tempCamera.cullingMask = drawOverLayers & ~(1<<4);
						tempCamera.targetTexture = rt;
						tempCamera.clearFlags = myCam.clearFlags==CameraClearFlags.Nothing?CameraClearFlags.SolidColor:myCam.clearFlags;
						tempCamera.backgroundColor = myCam.backgroundColor;
						tempCamera.backgroundColor = alphaBackground?Color.clear:myCam.backgroundColor;
						#if UNITY_5_6_OR_NEWER
						tempCamera.allowHDR = myCam.allowHDR;
						tempCamera.allowMSAA = myCam.allowMSAA;
						#else
						tempCamera.hdr = myCam.hdr;
						#endif
						tempCamera.targetTexture = mask;
						tempCamera.transform.position = myCam.transform.position;
						tempCamera.Render();
						m.SetTexture("_BackgroundReflection",mask);
					}
				}
				m.SetFloat("_AlphaBackground",alphaBackground?0:1);
				m.SetFloat("_SurfaceLevel", surfaceLevel );
			
				GetComponent<Renderer>().SetPropertyBlock(m);
			}

			tempCamera.targetTexture = null;

		}
	}


	public void SRPUpdate ( Camera cam ) {

		if ( !tempCamera ){
			tempCamera = new GameObject("TempPIDI2D_Camera", typeof(Camera) ).GetComponent<Camera>();
			tempCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
			tempCamera.enabled = false;
		}

		var downScale = 1.0f/downScaleValue;

		var myCam = cam;
		

		if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection") ){
			myCam = foregroundCam;
		}

		if ( !myCam || myCam.name == "TempPIDI2D_Camera" ){
			return;
		}
		
		if ( !rt ){
			rt = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}
		else if ( (int)(myCam.pixelWidth*downScale) != rt.width || (int)(myCam.pixelHeight*downScale) != rt.height ){
			DestroyImmediate(rt);
			rt = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}

		if ( !mask ){
			mask = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}
		else if ( (int)(myCam.pixelWidth*downScale) != rt.width || (int)(myCam.pixelHeight*downScale) != rt.height ){
			DestroyImmediate(rt);
			mask = new RenderTexture((int)(myCam.pixelWidth*downScale),(int)(myCam.pixelHeight*downScale), 0);
		}

		

		if ( myCam ){
			tempCamera.transform.position = myCam.transform.position;
			tempCamera.transform.rotation = myCam.transform.rotation;
			tempCamera.orthographic = myCam.orthographic;
			tempCamera.orthographicSize = myCam.orthographicSize;
			tempCamera.fieldOfView = myCam.fieldOfView;
			tempCamera.aspect = myCam.aspect;
			tempCamera.cullingMask = renderLayers & ~(1<<4);
			tempCamera.targetTexture = rt;
			tempCamera.clearFlags = myCam.clearFlags==CameraClearFlags.Nothing?CameraClearFlags.SolidColor:myCam.clearFlags;
			tempCamera.backgroundColor = myCam.backgroundColor;
			tempCamera.backgroundColor = alphaBackground||GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection")?Color.clear:myCam.backgroundColor;
			#if UNITY_5_6_OR_NEWER
			tempCamera.allowHDR = myCam.allowHDR;
			tempCamera.allowMSAA = myCam.allowMSAA;
			#else
			tempCamera.hdr = myCam.hdr;
			#endif

			if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_Reflection2D") ){
				tempCamera.Render();

				MaterialPropertyBlock m = new MaterialPropertyBlock();
				GetComponent<Renderer>().GetPropertyBlock( m );
				if ( surfaceLevel == -99 ){
					surfaceLevel = GetComponent<Renderer>().sharedMaterial.GetFloat("_SurfaceLevel");
				}

				if ( refColor == Color.clear ){
					refColor = GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
				}

				if ( backgroundColor == Color.clear && GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection") ){
					backgroundColor = GetComponent<Renderer>().sharedMaterial.GetColor("_ColorB");
				}

				m.SetTexture( "_Reflection2D", rt );

				if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_ReflectionMask") ){
					tempCamera.clearFlags = CameraClearFlags.SolidColor;
					tempCamera.backgroundColor = Color.clear;
					tempCamera.cullingMask = drawOverLayers;
					tempCamera.targetTexture = mask;
					tempCamera.transform.position = myCam.transform.position;
					tempCamera.Render();
					m.SetTexture("_ReflectionMask",mask);
				}
				//GetComponent<Renderer>().sharedMaterial.SetTexture( "_Reflection2D", rt );
				m.SetColor("_Color",refColor);
				if ( GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection") ){
					m.SetColor("_ColorB",backgroundColor);
					myCam = backgroundCam;
					if ( myCam ){
						tempCamera.transform.position = myCam.transform.position;
						tempCamera.transform.rotation = myCam.transform.rotation;
						tempCamera.orthographic = myCam.orthographic;
						tempCamera.orthographicSize = myCam.orthographicSize;
						tempCamera.fieldOfView = myCam.fieldOfView;
						tempCamera.aspect = myCam.aspect;
						tempCamera.cullingMask = drawOverLayers & ~(1<<4);
						tempCamera.targetTexture = rt;
						tempCamera.clearFlags = myCam.clearFlags==CameraClearFlags.Nothing?CameraClearFlags.SolidColor:myCam.clearFlags;
						tempCamera.backgroundColor = myCam.backgroundColor;
						tempCamera.backgroundColor = alphaBackground?Color.clear:myCam.backgroundColor;
						#if UNITY_5_6_OR_NEWER
						tempCamera.allowHDR = myCam.allowHDR;
						tempCamera.allowMSAA = myCam.allowMSAA;
						#else
						tempCamera.hdr = myCam.hdr;
						#endif
						tempCamera.targetTexture = mask;
						tempCamera.transform.position = myCam.transform.position;
						tempCamera.Render();
						m.SetTexture("_BackgroundReflection",mask);
					}
				}
				m.SetFloat("_AlphaBackground",alphaBackground?0:1);
				m.SetFloat("_SurfaceLevel", surfaceLevel );
			
				GetComponent<Renderer>().SetPropertyBlock(m);
			}

			tempCamera.targetTexture = null;

		}
	}


	void OnDestroy(){
		#if UNITY_2018_1_OR_NEWER && UNITY_EDITOR
		UnityEditor.EditorApplication.update -= LateUpdate;
		#endif

		if ( rt ){
			DestroyImmediate(rt);
		}
		if ( tempCamera ){
			DestroyImmediate(tempCamera.gameObject);
		}

	}

	void OnDisable(){

		#if UNITY_2018_1_OR_NEWER && UNITY_EDITOR
		UnityEditor.EditorApplication.update -= LateUpdate;
		#endif

		if ( rt ){
			DestroyImmediate(rt);
		}
		if ( tempCamera ){
			DestroyImmediate(tempCamera.gameObject);
		}

	
	}
}

