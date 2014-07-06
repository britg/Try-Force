using System.Threading;
using UnityEngine;
using System;
using System.IO;
using Launchie;

public class LaunchieLogic2 : MonoBehaviour
{
	// location of patches
	public string url;
	
	// current version of game
	public string version;
	
	// name of executable
	public string executable;
	
	double _progress;
	Launchie.Launchie _l;
	LaunchieGUI2 lgui;
	
	ManualResetEvent errorEvent = new ManualResetEvent(false);
	Exception errorException;
	
	void Start ()
	{
		if (Application.isEditor) {
			Debug.LogWarning ("Launchie mustn't run from editor!");
			return;
		}
		
		if ( url == null || url == "" || version == null || version == "" ) {
			Debug.LogWarning ("Launchie `url` and `version` cannot be empty!");
			return;
		}
		
		lgui = (LaunchieGUI2)GetComponent("LaunchieGUI2");
		
		new Thread (Asyncwork).Start();
	}
	
	void Asyncwork ()
	{
		_l = new Launchie.Launchie (url, version, executable);
		_l.debug = true;
		_l.setOnError( OnError );

		int check_state = _l.Check ();
		
		if (check_state == 1)
		{
			lgui.setState(1);
			DownloadPatch();
		}
		else if (check_state == 0)
		{
			lgui.setState(2);
			lgui.setText( "Game is up to date :)" );
			// there are no updates and you can load your game levels
			// here you can add something like this
			// Application.Loadlevel(1);
		}
		
		errorEvent.WaitOne ();
		
		if (errorException != null) {
			// handle any errors
		}
	}
	
	void DownloadReleaseNotesDone ()
	{
		lgui.setText( "There is update: " + _l.getAvailableVersion() + "; "+ FormatSize(_l.getSize()) + "\n" + _l.getReleaseNotes() );
	}
	
	public void DownloadPatch ()
	{
		if (lgui.getState() == 1) {
			_l._onProgress += DownloadPatchProgress;
			_l.setOnDone( DownloadPatchDone );
			
			lgui.setState(3);
			lgui.setText( "Downloading " + _l.getAvailableVersion() + "; "+ FormatSize(_l.getSize()) + "\n" + _l.getReleaseNotes() );
			_l.Download ();
		}
	}
	
	string FormatSize( double size )
	{
		string unit = "B";
		if( size > 921 ) // 1024 * 90%
		{
			unit = "KB";
			size /= 1024;
		}
		if( size > 921 ) // 1024 * 1024 * 90%
		{
			unit = "MB";
			size /= 1024;
		}
		if( size > 921 ) // 1024 * 1024 * 1024 * 90%
		{
			unit = "GB";
			size /= 1024;
		}
		
		return Math.Round( size, 2 ) + unit;
	}
	
	void DownloadPatchDone ()
	{
		_progress = 0;
		lgui.setProgress(_progress);
		_l._onProgress += ExtractProgress;
		_l.setOnDone( ExtractDone );
		
			
		lgui.setState(4);
		lgui.setText( "Unpacking " + _l.getAvailableVersion() + "; "+ FormatSize(_l.getSize()) + "\n" + _l.getReleaseNotes() );
		_l.Extract ();
	}
	
	
	DateTime last_time_speed_check = DateTime.Now;
	double last_time_progress ;
	string speed;
	void DownloadPatchProgress ( double progress )
	{
		lgui.setProgress(progress);
		
		double diff = (DateTime.Now - last_time_speed_check).TotalMilliseconds;
		
		if( diff > 100 )
		{
			last_time_speed_check = DateTime.Now;
			speed = FormatSize((progress - last_time_progress) / 100 * _l.getSize() * 1000 / diff) + "/s";
			last_time_progress = progress;
		}
		
		
		// if you set this to false it will count from 0 to size of patch
		// if true it will show remaining bytes
		bool remaining_size = true;
		if( remaining_size )
		{
			lgui.setText( "Downloading " + _l.getAvailableVersion() + "; "+speed+"; "+ FormatSize( _l.getSize() * (100 - progress) / 100) + "\n" + _l.getReleaseNotes() );
		}
		else
		{
			lgui.setText( "Downloading " + _l.getAvailableVersion() + "; "+speed+"; "+ FormatSize( _l.getSize() * ( progress / 100) ) + "\n" + _l.getReleaseNotes() );
		}
	}
	
	void ExtractProgress ( double progress )
	{
		lgui.setProgress(progress);
	}
	
	void ExtractDone ()
	{
		_l.setCurrentVersion ( _l.getAvailableVersion () );
		if( _l.ForceCheck() == 1 )
		{
			lgui.setState(1);
			DownloadPatch();
		}
		else
		{
			_l.Finish ();
			lgui.setState(5);
		}
	}
	
	void OnError (Exception ex)
	{
		errorException = ex;
		errorEvent.Set ();
	}
}