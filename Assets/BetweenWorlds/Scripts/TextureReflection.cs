using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class TextureReflection : MonoBehaviour
{
    public Camera _camera;
    private RenderTexture _renderTexture;
    private Texture2D _textureBuffer;
    private MeshRenderer _meshRenderer;
    private VXComponent _vxc;
    
    // Start is called before the first frame update
    void Start()
    {

        _renderTexture = _camera.targetTexture;
        
        _textureBuffer = new Texture2D((int) _renderTexture.width, (int) _renderTexture.height)
        {
            name = _renderTexture.name,
        };
        
        // Assign Output Texture
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material.mainTexture = _textureBuffer;
    }

    // Update is called once per frame
    void Update()
    {
        if (_renderTexture == null && _camera.targetTexture)
        {
            _renderTexture = _camera.targetTexture;
        }
        
        if (_vxc != null && _camera.targetTexture) // Ensure VXComponent is present
        {
            if (_textureBuffer.width != _renderTexture.width || _textureBuffer.height != _renderTexture.height)
            {
                Debug.Log("Resize Texture");
                _textureBuffer.Resize(_renderTexture.width, _renderTexture.height);
            }
            
            RenderTexture.active = _renderTexture;
            _textureBuffer.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            _textureBuffer.Apply();
            RenderTexture.active = null;
            
            _vxc.RefreshDynamicTexture(ref _textureBuffer);
        }
        else
        {
            _vxc = GetComponent<VXComponent>();
        }
    }

    private void LateUpdate()
    {
        _camera.Render();
    }
}
