using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System;

public class Timer : MonoBehaviour {

    private float timeToWrite = 300.0f;
    private float yearTime = 24.0f;
    int dayInYear = 366;
    float minInSec;
    public int years;
    public int days;
    public int hours;
    public float minutes;

    string path;
    string directoryName = "Saves";
    string mainPath;
    FileStream myFile;
    string fileName = "LastGameTime.txt";
    string fileDirectory;

    private float dayCycleInMinutes;
    private float nightCycleInMinutes;

    private const float SECOND = 1;
    private const float MINUTE = 60 * SECOND;
    private const float HOUR = 60 * MINUTE;
    private const float DAY = 24 * HOUR;

    private float _degreeRotationInDayTime;
    private float _degreeRotationInNightTime;

    public float _degreeRotation;

    private int sunRiseTime = 4;
    private int sunSetTime = 21;
    private int nightTime;

	// Use this for initialization
	void Start () {
        nightTime = sunRiseTime + 24 - sunSetTime;

        minInSec = (float)((dayInYear) / yearTime)*24 / 60;
        path = Application.dataPath + "/";
        mainPath = path + directoryName + "/";
        if (!Directory.Exists(mainPath))
        {
            Directory.CreateDirectory(mainPath);
        }
        fileDirectory = mainPath + fileName;
        if (!File.Exists(fileDirectory))
        {
            years = 0;
            days = 0;
            hours = 0;
            minutes = 0;

            myFile = File.Create(fileDirectory);
            AddInfo(myFile, years.ToString());
            AddInfo(myFile, days.ToString());
            AddInfo(myFile, hours.ToString());
            AddInfo(myFile, minutes.ToString());
        }
        else
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileDirectory))
                {
                    String line = sr.ReadLine();
                    years = Int32.Parse(line);
                    line = sr.ReadLine();
                    days = Int32.Parse(line);
                    line = sr.ReadLine();
                    hours = Int32.Parse(line);
                    line = sr.ReadLine();
                    minutes = Int32.Parse(line);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("The file could not be read:" + e.Message);
            }
        }

        dayCycleInMinutes = (24 - nightTime) / minInSec;
        nightCycleInMinutes = nightTime / minInSec;

        _degreeRotationInDayTime = 180 / (dayCycleInMinutes * MINUTE);
        _degreeRotationInNightTime = 180 / (nightCycleInMinutes * MINUTE);

        if (hours >= sunSetTime || hours < sunRiseTime)
            _degreeRotation = _degreeRotationInNightTime;
        if (hours >= sunRiseTime && hours <= sunSetTime)
            _degreeRotation = _degreeRotationInDayTime;
        SetSun();
        WriteTime();
	}
	
	// Update is called once per frame
	void Update () {
        minutes += Time.deltaTime * minInSec;
        transform.Rotate(new Vector3(_degreeRotation,0,0)*Time.deltaTime);
        if (minutes >= 60)
        {
            hours++;
            if (hours >= sunSetTime || hours < sunRiseTime)
                _degreeRotation = _degreeRotationInNightTime;
            if (hours >= sunRiseTime && hours < sunSetTime)
                _degreeRotation = _degreeRotationInDayTime;
            minutes -= 60;
        }
        if (hours >= 24)
        {
            days++;
            hours -= 24;
        }
        if (days >= dayInYear)
        {
            years++;
            days -= dayInYear-1;
        }
        if (days == 0)
        {
            days = 1;
        }
	}
    void SetSun()
    {
        if (hours >= sunSetTime || hours < sunRiseTime)
            transform.rotation = Quaternion.Euler((hours * 60 + minutes - 240) * 0.42857142857142857142857142857143f, 0, 0);
        if (hours >= sunRiseTime && hours < sunSetTime)
            transform.rotation = Quaternion.Euler((hours * 60 + minutes - 240) * 0.17647058823529411764705882352941f, 0, 0);

    }
    void OnGUI()
    {
        GUI.Box(new Rect(100, 5, 100, 25), ((int)years).ToString());
        GUI.Box(new Rect(200, 5, 100, 25), ((int)days).ToString());
        GUI.Box(new Rect(300, 5, 100, 25), ((int)hours).ToString());
        GUI.Box(new Rect(400, 5, 100, 25), ((int)minutes).ToString());
    }

    void AddInfo(FileStream fs, string text)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(text);
        sb.AppendLine();
        byte[] info = new UTF8Encoding(true).GetBytes(sb.ToString());
        fs.Write(info, 0, info.Length);
    }
    void WriteTime()
    {
        StartCoroutine(WTime());
    }
    IEnumerator WTime()
    {

        yield return new WaitForSeconds(timeToWrite);

        myFile = File.OpenWrite(fileDirectory);
        AddInfo(myFile, ((int)years).ToString());
        AddInfo(myFile, ((int)days).ToString());
        AddInfo(myFile, ((int)hours).ToString());
        AddInfo(myFile, ((int)minutes).ToString());
        myFile.Close();
        WriteTime();
    }
    void OnApplicationQuit()
    {
        myFile = File.OpenWrite(fileDirectory);
        AddInfo(myFile, ((int)years).ToString());
        AddInfo(myFile, ((int)days).ToString());
        AddInfo(myFile, ((int)hours).ToString());
        AddInfo(myFile, ((int)minutes).ToString());
        myFile.Close();
    }
}
