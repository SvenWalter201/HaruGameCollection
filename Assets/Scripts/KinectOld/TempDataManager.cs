using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

class TempDataManager : Singleton<TempDataManager>
{
    private long nextHandle = 0L;
    private Dictionary<long, TimedCollection> allCollections; 


    public long CreateNewDataCollection(long maximumLifetime)
    {
        long handle = nextHandle;
        allCollections.Add(handle, new TimedCollection(maximumLifetime));
        nextHandle++;
        return handle;
    }

    public void DeleteDataCollection(long handle)
    {
        allCollections.Remove(handle);
    } 

    public void AddDataToCollection(long handle, Vector3 vec)
    {
        
        allCollections[handle].AddData(vec);
    }

    public TimedCollection GetCollection(long handle)
    {
        return allCollections[handle];
    }

    public Vector3 GetAverageBetweenFromCollection(long handle, long from, long to)
    {
        return allCollections[handle].GetAverageBetween(from, to);
    }

    public void ClearCollection(long handle)
    {
        allCollections[handle].Clear();
    }
    
}

class TimedCollection
{
    public long m_maximumLifetime;
    public List<TimeStampedVector> m_data;

    public TimedCollection(long maximumLifetime)
    {
        m_maximumLifetime = maximumLifetime;
        m_data = new List<TimeStampedVector>();
    }

    public TimedCollection(long maximumLifetime, List<TimeStampedVector> data)
    {
        m_maximumLifetime = maximumLifetime;
        m_data = data;
    }


    public void AddData(Vector3 vec)
    {
        lock (this)
        {
            //check, if the list is 'full'
            if (m_data.Count > (m_maximumLifetime / 1000) * 30)
            {
                CleanUp();
            }
            m_data.Add(new TimeStampedVector
            {
                m_data = vec,
                m_timeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond
            });
        }

    }

    public void Clear()
    {
        m_data.Clear();
    }

    public Vector3 GetAverageAll()
    {
        CleanUp();
        Vector3 sum = new Vector3();
        foreach (TimeStampedVector tsv in m_data)
        {
            sum += tsv.m_data;
        }
        sum /= m_data.Count;
        return sum;
    }

    //get the average of all values between two timestamps
    public Vector3 GetAverageBetween(long from, long to)
    {
        lock (this)
        {
            CleanUp();
            Vector3 sum = new Vector3();
            int count = 0;
            foreach (TimeStampedVector tsv in m_data)
            {
                //Console.WriteLine("timestamp: " + tsv.m_timeStamp);

                if (tsv.m_timeStamp < from)
                {
                    continue;
                }
                if (tsv.m_timeStamp > to)
                {
                    break;
                }
                count++;
                sum += tsv.m_data;
            }
            sum /= count;
            return sum;
        }
    }



    public void CleanUp()
    {
        long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        for (int i = 0; i < m_data.Count; i++)
        {
            if (currentTime - m_data[i].m_timeStamp > m_maximumLifetime)
            {
                m_data.RemoveAt(i);
            }
        }
    }
}

struct TimeStampedVector
{
    public long m_timeStamp;
    public Vector3 m_data;
} 

