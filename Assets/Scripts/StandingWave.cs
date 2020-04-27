﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class StandingWave : MonoBehaviour
{
    [SerializeField] private UserControl userControl;
    [SerializeField] private Camera _camera;
    [SerializeField] private LineRenderer baseline;
    private LineRenderer _wave;
    private const int linePoints = 128;
    private bool active;

    // animation params, set by another script
    public Dim waveDim;
    public Vector3Int currentOrder;
    public Vector3 pos1;
    public Vector3 pos2;
    public float freq;
    float[] waveVals;
    const float amplitude = 0.3f;
    const float alpha = 0.3f;

    private void Start()
    {
        _wave = GetComponent<LineRenderer>();
        _wave.enabled = false;
        _wave.positionCount = 128;
        baseline.enabled = false;
        baseline.positionCount = 2;
        waveDim = Dim.Length;
        currentOrder = new Vector3Int(0, 0, 1);
        waveVals = new float[linePoints];
    }

    private void Update()
    {
        if (active)
        {
            //Animate();
        }
        
    }

    public void SetActive(bool b)
    {
        active = b;
        _wave.enabled = b;
        baseline.enabled = b;
    }

    // One-time prep whenever mode changes
    public void InitializeAnimation()
    {
        SetActive(true);
        // update freq
        if (userControl.roomModel.modeDict.TryGetValue(currentOrder, out float f))
        {
            freq = f;
        }
        // update end positions
        switch (waveDim)
        {
            case Dim.Length:
                pos1 = userControl.roomModel.roomPoints.len1;
                pos2 = userControl.roomModel.roomPoints.len2;
                break;
            case Dim.Width:
                pos1 = userControl.roomModel.roomPoints.wid1;
                pos2 = userControl.roomModel.roomPoints.wid2;
                break;
            case Dim.Height:
                pos1 = userControl.roomModel.roomPoints.hgt1;
                pos2 = userControl.roomModel.roomPoints.hgt2;
                break;
        }
        baseline.SetPosition(0, pos1);
        baseline.SetPosition(1, pos2);
        // todo prepare a full wave setup
        int maxOrder = currentOrder.x + currentOrder.y + currentOrder.z;
        CalculateCosineForOrder(maxOrder);
        Animate();
    }

    public void Animate()
    {
        // change the positions of the line renderer
        for (int i = 0; i < linePoints; i++)
        {
            float pct = i / (float)linePoints;
            Vector3 pos = pos1 + ((pos2-pos1) * pct);
            if (currentOrder.y > 0)
                pos += _camera.transform.right * amplitude * waveVals[i];
            else
                pos += _camera.transform.up * amplitude * waveVals[i];
            _wave.SetPosition(i, pos);
        }

        _wave.startColor = new Color((1 + waveVals[0])/2, 0.1f, (1 - waveVals[0]) / 2, alpha);
        _wave.endColor = new Color((1 + waveVals[linePoints - 1]) / 2, 0.1f, (1 - waveVals[linePoints - 1]) / 2, alpha);
        _wave.colorGradient = new Gradient()
    }

    public void ToggleDimension()
    {
        waveDim++;
        if (waveDim == Dim.none)
            waveDim = Dim.Length;
        switch(waveDim)
        {
            case Dim.Length:
                currentOrder = new Vector3Int(0, 0, 1);
                break;
            case Dim.Width:
                currentOrder = new Vector3Int(1, 0, 0);
                break;
            case Dim.Height:
                currentOrder = new Vector3Int(0, 1, 0);
                break;
        }
        InitializeAnimation();
    }

    public void IncrementOrder()
    {
        switch (waveDim)
        {
            // try to change the order, but revert if not possible
            case Dim.Length:
                currentOrder.z++;
                if (!CanChangeOrder(currentOrder))
                { currentOrder.z--; }
                break;
            case Dim.Width:
                currentOrder.x++;
                if (!CanChangeOrder(currentOrder))
                { currentOrder.x--; }
                break;
            case Dim.Height:
                currentOrder.y++;
                if (!CanChangeOrder(currentOrder))
                { currentOrder.y--; }
                break;
        }
        InitializeAnimation();
    }

    public void DecrementOrder()
    {
        switch (waveDim)
        {
            // try to change the order, but revert if not possible
            case Dim.Length:
                currentOrder.z--;
                if (!CanChangeOrder(currentOrder))
                {   currentOrder.z++;   }
                break;
            case Dim.Width:
                currentOrder.x--;
                if (!CanChangeOrder(currentOrder))
                { currentOrder.x++; }
                break;
            case Dim.Height:
                currentOrder.y--;
                if (!CanChangeOrder(currentOrder))
                { currentOrder.y++; }
                break;
        }
        InitializeAnimation();
    }

    // returns true when there is a mode in roomModel for given order
    private bool CanChangeOrder(Vector3Int newOrder)
    {
        if (userControl.roomModel.modeDict.TryGetValue(newOrder, out _))
        { return true; }
        else return false;
    }

    private void CalculateCosineForOrder(int n)
    {
        for (int i = 0; i < linePoints; i++)
        {
            float phase = Mathf.PI * n * i / linePoints;
            waveVals[i] = Mathf.Cos(phase);
        }
    }

    private Gradient MakeColorGradientForOrder(int n)
    {
        Gradient g = new Gradient();
        n++;




        return g;

    }

}
