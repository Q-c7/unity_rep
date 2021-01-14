using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SQLite;
using System;

public struct AlarmRowItem
{
    public DateTime time;
    public string text;
}

public class Alarms : MonoBehaviour
{
    SQLiteConnection connection;
    public bool stateGood;
    public string string_FILE = "alarms.db";
    public delegate void NewEvent(int id);
    public event NewEvent OnNewEvent;
    // Start is called before the first frame update
    void Start()
    {        
        
    }

    private void Awake()
    {
        if (connection == null)
            InitConnection();
        try
        {
            stateGood = false;
            connection.Open();
        }
        finally
        {
            stateGood = true;
        }

        using (var cmd = new SQLiteCommand(connection))
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS log_texts (" +
            "id INTEGER PRIMARY KEY," +
            "shortText varchar," +
            "text varchar" +
            ")";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS logs (" +
                "id INTEGER PRIMARY KEY," +
                "cameIn datetime," +
                "entrytype integer," +
                "ack bool," +
                "ackTime datetime," +
                "cameOut bool," +
                "cameOutTime datetime," +
                "alarmId integer," +
                "logText varchar" +
                ")";
            cmd.ExecuteNonQuery();
        };
    }

    private void InitConnection()
    {
        var csb = new SQLiteConnectionStringBuilder
        {
            DataSource = string_FILE,
            Version = 3
        };
        connection = new SQLiteConnection(csb.ConnectionString);
    }

    public void Alarm(string text, int entryType = 0) // entryType - ВОЗМОЖНОСТЬ ГРУППИРОВАТЬ
    {
        var query = "INSERT INTO logs (cameIn, entryType, ack, cameOut, logText) VALUES " +
            "(CURRENT_TIMESTAMP, {0}, false, false, '{1}')";
        using (var cmd = new SQLiteCommand(connection))
        {
            cmd.CommandText = string.Format(query, entryType, text);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT last_insert_rowid()";
            object tmp = cmd.ExecuteScalar();
            long id = 0;
            if (tmp != DBNull.Value && tmp != null)
                id = (long)tmp;
            OnNewEvent?.Invoke((int)id);
        }
    }

    public List<AlarmRowItem> GetAlarms()
    {
        using (var cmd = new SQLiteCommand(connection))
        {
            var alarmsList = new List<AlarmRowItem>();
            cmd.CommandText = "SELECT cameIn, logText FROM logs ORDER BY cameIn DESC LIMIT 200";
            using(var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var time = reader.GetDateTime(0);
                        var diff = time.Subtract(time.ToUniversalTime());
                        var logText = reader.GetString(1);
                        alarmsList.Add(new AlarmRowItem() { time = time.Add(diff), text = logText });
                    }
                }
                
            }
            return alarmsList;
        }
    }

    // Update is called once per frame
    void Update()
    {







    }
}
