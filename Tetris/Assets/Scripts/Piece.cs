using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetColor(MinoDef.MinoType inType)
    {
        if(spriteRenderer == null)
        {
            return;
        }
        switch (inType)
        {
            case MinoDef.MinoType.TMino:
                {
                    // 紫色に設定
                    spriteRenderer.color = new Color32(0x65, 0x51, 0xA1, 0xFF);
                }
                break;
            case MinoDef.MinoType.SMino:
                {
                    // 緑色に設定
                    spriteRenderer.color = new Color32(0x84, 0xC9, 0x8B, 0xFF);
                }
                break;
            case MinoDef.MinoType.ZMino:
                {
                    // 赤色に設定
                    spriteRenderer.color = new Color32(0xED, 0x1A, 0x3D, 0xFF);
                }
                break;
            case MinoDef.MinoType.LMino:
                {
                    // 橙色に設定
                    spriteRenderer.color = new Color32(0xF1, 0x5A, 0x22, 0xFF);
                }
                break;
            case MinoDef.MinoType.JMino:
                {
                    // 青色に設定
                    spriteRenderer.color = new Color32(0x00, 0x67, 0xC0, 0xFF);
                }
                break;
            case MinoDef.MinoType.IMino:
                {
                    // 水色に設定
                    spriteRenderer.color = new Color32(0x65, 0xBB, 0xE9, 0xFF);
                }
                break;
            case MinoDef.MinoType.OMino:
                {
                    // 黃色に設定
                    spriteRenderer.color = new Color32(0xFF, 0xC8, 0x00, 0xFF);
                }
                break;
        }
    }
}
