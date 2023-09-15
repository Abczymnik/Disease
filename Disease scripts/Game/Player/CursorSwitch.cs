using UnityEngine;

public class CursorSwitch : MonoBehaviour
{
    public static CursorSwitch CursorSkin { get; private set; }

    private static Texture2D standardTex;
    private static Sprite standardSprite;
    private static Texture2D noteTex;
    private static Sprite noteSprite;
    private static Texture2D optionsTex;
    private static Sprite optionsSprite;
    private static Texture2D attackTex;
    private static Sprite attackSprite;
    private static SpriteRenderer spriteRend;

    private static string actualSkin;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (CursorSkin != null && CursorSkin != this)
        {
            Destroy(this);
        }
        else
        {
            CursorSkin = this;
            Cursor.visible = false;
            LoadSprites();
            LoadTextures();
            SetSpriteRenderer();
        }
    }

    private void LoadSprites()
    {
        standardSprite = Resources.Load<Sprite>("Cursors/Arrow");
        noteSprite = Resources.Load<Sprite>("Cursors/Cursor_book");
        optionsSprite = Resources.Load<Sprite>("Cursors/Cursor_Settings");
        attackSprite = Resources.Load<Sprite>("Cursors/Curor_Attack");
    }

    private void LoadTextures()
    {
        standardTex = Resources.Load<Texture2D>("Cursors/Arrow");
        noteTex = Resources.Load<Texture2D>("Cursors/Cursor_book");
        optionsTex = Resources.Load<Texture2D>("Cursors/Cursor_Settings");
        attackTex = Resources.Load<Texture2D>("Cursors/Curor_Attack");
    }

    private void SetSpriteRenderer()
    {
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public static void SwitchSkin(string cursorSkin)
    {
        if (cursorSkin == actualSkin) return;
        actualSkin = cursorSkin;

        switch (cursorSkin)
        {
            case "standard":
                spriteRend.sprite = standardSprite;
                spriteRend.material.mainTexture = standardTex;
                break;
            case "note":
                spriteRend.sprite = noteSprite;
                spriteRend.material.mainTexture = noteTex;
                break;
            case "options":
                spriteRend.sprite = optionsSprite;
                spriteRend.material.mainTexture = optionsTex;
                break;
            case "attack":
                spriteRend.sprite = attackSprite;
                spriteRend.material.mainTexture = attackTex;
                break;
            default:
                spriteRend.sprite = standardSprite;
                spriteRend.material.mainTexture = standardTex;
                break;
        }
    }

    public static void HideCursor()
    {
        if (spriteRend == null) return;
        spriteRend.enabled = false;
    }

    public static void ShowCursor()
    {
        if (spriteRend == null) return;
        spriteRend.enabled = true;
    }    
}
