using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Dissonance.Editor
{
    // ReSharper disable once InconsistentNaming
    public class VUMeter
    {
        private float _smooth;
        private float _maxVolume;
        private float _peak;

        private readonly Stopwatch _time;

        public VUMeter()
        {
            _time = new Stopwatch();
        }

        public void DrawInspectorGui(float amplitude, bool clear)
        {
            // Keep track of time between calls
            var dt = (float)_time.Elapsed.TotalSeconds;
            _time.Reset();
            _time.Start();

            // Smooth out the signal slightly by mixing it with the previous value
            _smooth = amplitude * 0.3f + _smooth * 0.7f;

            // Stretch the meter to the largest signal ever encountered
            _maxVolume = Mathf.Max(_smooth, _maxVolume);

            // Reset if necessary
            if (clear)
            {
                _smooth = 0;
                _maxVolume = 0;
                _peak = 0;
            }

            // Update peak volume
            if (_smooth > _peak)
                _peak = _smooth;
            else
                _peak -= dt * 0.025f;
            _peak = Mathf.Clamp01(_peak);

            // Draw background
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.DrawRect(rect, Color.gray);

            // Draw current amplitude indicator
            var c = Color.HSVToRGB(Mathf.Lerp(0.5f, 0, _smooth / _maxVolume), 0.8f, 0.8f);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width * _smooth / _maxVolume, rect.height), c);
            
            // Draw peak amplitude indicator
            var x = rect.width * _peak / _maxVolume;
            EditorGUI.DrawRect(new Rect(rect.xMin + x - 1, rect.yMin, 2, rect.height), Color.red);
        }
    }
}
