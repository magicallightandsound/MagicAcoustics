﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HeightViewState : State
{
    public HeightViewState(UserControl userControl, Text header, List<Text> columns) : base(userControl, header, columns)
    {
        myState = StateEnum.HeightView;
    }

    public override void Tick()
    {

    }

    public override void OnStateEnter()
    {
        GLOBALS.isMeshing = false;
        GLOBALS.measuringDim = Dim.none;
        userControl.EnableBeam(false);
        header.text = "HEIGHT: " + userControl.roomModel.dimensions.y.ToString(GLOBALS.format) + " ft\n" +
                        "Continue?";
    }

    public override void OnStateExit()
    {
        ClearText();

    }

    public override void OnTriggerUp()
    {
        // advance to analyses
        userControl.roomModel.CalcModes();
        userControl.roomModel.CalcDimensions();
        userControl.SetState(new ModeListState(userControl, header, columns));
    }

    public override void OnBumperUp()
    {
        userControl.SetState(new HeightState(userControl, header, columns));
    }

    public override void OnHomeUp()
    {
        userControl.SetState(new RoomScanState(userControl, header, columns));
    }

}