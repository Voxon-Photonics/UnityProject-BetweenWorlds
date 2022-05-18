using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = UnityEngine.Random;

public class TouchMenu : MonoBehaviour
{
    private bool _initialised = false;
    
    /* Menu Values */
    static float yHeight = 0.0f, vSlider = 0; 
    static int menuChoice = 0, choiceCol = 0x00ff00;
    static int hSlider = 0, goColour = 0xffffff;
    static string testMessage = "Edit me using the menu!";
    public enum MENU
    {
        //Generic names
        UP, ONE, TWO, THREE, DOWN, VSLIDERNAM, HSLIDERNAM,
        GO, EDITDO_ID, EDIT_ID, 
	
        //Other
        AUTOROTATEOFF, AUTOROTATEX, AUTOROTATEY, AUTOROTATEZ, AUTOROTATESPD,
        FRAME_PREV, FRAME_NEXT,
        SOL0, SOL1, SOL2, SOL3,
        HINT, DIFFICULTY, SPEED,
        ZIGZAG, SINE, HUMP_LEVEL,
        DISP_CUR, DISP_END=DISP_CUR+2,
    }; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_initialised && VXProcess.Instance.active)
        {
            _initialised = true;
            VXProcess.Runtime.MenuReset(menu_update, IntPtr.Zero); // resets the menu tab
            VXProcess.Runtime.MenuAddTab("Test Name",		350,0,600,500);
            VXProcess.Runtime.MenuAddText((int)MENU_TYPE.MENU_TEXT,"Menu Text Test", 220,20, 64, 64,0); // adding menu text
            
            // single button (MENU_BUTTON+3)
            VXProcess.Runtime.MenuAddButton((int)MENU.UP,"Up",	400, 50,75,50,0x808000, (int)MENUPOSITION.SINGLE);
	
            // group of butons (first button MENU_BUTTON+1, middle button just MENU_BUTTON, last button MENU_BUTTON+2)
            VXProcess.Runtime.MenuAddButton((int)MENU.ONE,"1",300,110,75,50,0xFF0000, (int)MENUPOSITION.FIRST);
            VXProcess.Runtime.MenuAddButton((int)MENU.TWO,"2",400,110,75,50,0x00FF00, (int)MENUPOSITION.MIDDLE);
            VXProcess.Runtime.MenuAddButton((int)MENU.THREE,"3",500,110,75,50,0x0000FF, (int)MENUPOSITION.END);

            // single button
            VXProcess.Runtime.MenuAddButton((int)MENU.DOWN,"Down",	400,170,75,50,0x008080, (int)MENUPOSITION.SINGLE);

            // vertical slider the values at the end after the colour value are l
            // sider settings are after the colour value is starting starting value, lowest value, highest valuer, minimal adjustment, major adjustment
            VXProcess.Runtime.MenuAddVerticleSlider((int)MENU.VSLIDERNAM, "V Slider", 75,85,64,150, 0xFFFF80, 0.5f, 2.0f, 0.1f, 0.1f, 0.3f);

            // horizontal slider
            // sider settings are after the colour value is starting starting value, lowest value, highest valuer, minimal adjustment, major adjustment
            VXProcess.Runtime.MenuAddHorizontalSlider((int)MENU.HSLIDERNAM, "H Slider", 200,270,300,64, 0x808080, 5.0f, 1.0f, 20.0f, 1.0f, 3.0f);

            // How to input a string
            VXProcess.Runtime.MenuAddEdit((int)MENU.EDIT_ID, "Edit this text", 50,350,500,50, 0x808080);

            // Edit is like Edit Do does the next action when you press enter in this case will hit the Go button
            VXProcess.Runtime.MenuAddEdit((int)MENU.EDITDO_ID, "Edit Do", 50,410,400,50, 0x808080, true);

            // A button which is linked to Edit Do
            VXProcess.Runtime.MenuAddButton((int)MENU.GO,"GO",	460,410,90,50,	0x08FF80, (int)MENUPOSITION.SINGLE);

            // add some more text
            VXProcess.Runtime.MenuAddText((int)0,"Matthew Vecchio / ReadyWolf for Voxon 2019", 40,480, 64, 64,0xFF0000); // adding menu text
        }
    }
    
    static int menu_update (int id, string st, double v, int how, IntPtr userdata)
    {
        switch(id)
        {
            case (int)MENU.VSLIDERNAM: 
                vSlider = (float)v; break;
            case (int)MENU.HSLIDERNAM: 
                hSlider = (int)v; break;
            case (int)MENU.EDIT_ID:
            case (int)MENU.EDITDO_ID:
                testMessage = st.ToString(); break;
            case (int)MENU.UP:   
                yHeight -= 0.05f; break; 
            case (int)MENU.ONE:  
                menuChoice = 1;  choiceCol = 0xFF0000; break;
            case (int)MENU.TWO:  
                menuChoice = 2;  choiceCol = 0x00FF00;break;
            case (int)MENU.THREE: 
                menuChoice = 3; choiceCol = 0x0000FF; break;
            case (int)MENU.DOWN:  
                yHeight += 0.1f;  break;
            case (int)MENU.GO: 
                goColour = ((Random.Range(0,1)&1)<<7) + ((Random.Range(0,1)&1)<<15) + ((Random.Range(0,1)&1)<<23); break;
        }
        return(1);
    }
}
