using System.Threading;
using UnityEngine;
using System;

public class LaunchieGUI1 : MonoBehaviour
{
	string _text;
	int _guistate = 0;
	double _progress = 0;

	public Font font;
	
	LaunchieLogic1 llogic;
	
	public void setState( int guistate )
	{
		_guistate = guistate;
	}
	
	public int getState()
	{
		return _guistate;
	}
	
	public void setText( string text )
	{
		_text = text;
	}
	
	public void setProgress( double progress )
	{
		_progress = progress;
	}
	
	void Start ()
	{
		llogic = (LaunchieLogic1)GetComponent("LaunchieLogic1");
		_text = "Checking for updates...";
	}
	
	void OnGUI ()
	{
		if (Application.isEditor) {
			return;
		}

		GUI.skin.font = font;

		int x = (Screen.width - 300) / 2;
		int y = (Screen.height - 200) / 2;
		

		GUI.Box (new Rect (x, y, 300, 200), _text);
		if (_guistate == 1)
		{
			bool clicked = GUI.Button (new Rect (x + 50, y + 150, 200, 30), "Download");	
			if (clicked) {
				llogic.DownloadPatch();
			}
		} else if (_guistate == 3 || _guistate == 4) {
			GUI.Box (new Rect (x + 50, y + 150, 200, 30), Math.Round(_progress, 2) + "%");
			GUI.Box (new Rect (x + 50, y + 150, (float)Math.Max( 12, (2 * _progress) ), 30), "");
		} else if (_guistate == 5) {
			Application.Quit();
		}
	
	}
}