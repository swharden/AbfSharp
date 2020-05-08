﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ABFsharp
{
    public class AbfHeader
    {
        // info that does not change as sweeps/channels change

        public readonly string filePath;
        public readonly string fileName;
        public readonly string id;
        public readonly long fileSize;
        public readonly double fileSizeMb;
        //public readonly string[] adcNames;
        //public readonly string[] adcUnits;
        //public readonly string[] dacNames;
        //public readonly string[] dacUnits;

        // data
        public readonly int channelCount;
        public readonly int sweepCount;
        public readonly int tagCount;
        public readonly Tag[] tags;

        public readonly int sampleRate;
        public readonly double pointsPerMs;

        public readonly double totalLengthMin;
        public readonly double totalLengthSec;
        public readonly int totalLengthPoints;

        public readonly double sweepLengthMin;
        public readonly double sweepLengthSec;
        public readonly int sweepLengthPoints;

        public readonly double sweepIntervalSec;
        public readonly double sweepIntervalMin;

        // details from the header
        //public readonly DateTime dateTime;
        //public readonly AbfVersion version;
        //public readonly string comment;
        //public readonly string creator;
        //public readonly string creatorVersion;
        public readonly string protocolFilePath;
        public readonly string protocol;

        public AbfHeader(string abfFilePath, ABFFIO.AbfInterface abffio)
        {
            var header = abffio.header;

            filePath = System.IO.Path.GetFullPath(abfFilePath);
            fileName = System.IO.Path.GetFileName(abfFilePath);
            id = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);
            fileSize = new System.IO.FileInfo(abfFilePath).Length;
            fileSizeMb = (double)fileSize / 1e6;

            if (header.fADCSequenceInterval == 0)
                throw new Exception("abf header hasn't been properly initialized");

            sampleRate = (int)(1e6 / header.fADCSequenceInterval);
            pointsPerMs = (double)sampleRate / 1000;
            channelCount = header.nADCNumChannels;

            sweepCount = header.lActualEpisodes;
            sweepLengthPoints = header.lNumSamplesPerEpisode / channelCount;
            sweepLengthSec = (double)sweepLengthPoints / sampleRate;
            sweepLengthMin = sweepLengthSec / 60;
            sweepIntervalSec = header.fEpisodeStartToStart;
            if (sweepIntervalSec == 0)
                sweepIntervalSec = sweepLengthSec;
            sweepIntervalMin = sweepIntervalSec / 60;

            totalLengthSec = sweepIntervalSec * sweepCount;
            totalLengthMin = totalLengthSec / 60;
            totalLengthPoints = sweepLengthPoints * sweepCount;

            tagCount = header.lNumTagEntries;
            tags = new Tag[tagCount];

            protocolFilePath = header.sProtocolPath.Trim();
            if (protocolFilePath == "(untitled)")
                protocolFilePath = "";
            protocol = System.IO.Path.GetFileNameWithoutExtension(protocolFilePath);

            // read comment tags
            ABFFIO.Structs.ABFTag[] abfTags = abffio.ReadTags();
            for (int i = 0; i < abfTags.Length; i++)
            {
                ABFFIO.Structs.ABFTag abfTag = abfTags[i];
                double timeSec = abfTag.lTagTime * abffio.header.fSynchTimeUnit / 1e6;
                string comment = new string(abfTag.sComment).Trim();
                int timeSweep = (int)(timeSec / sweepIntervalSec);
                Tag tag = new Tag(timeSec, timeSweep, comment, abfTag.nTagType);
                tags[i] = tag;
            }

            // read epoch table
            for (int i = -1; i < ABFFIO.Structs.ABF_EPOCHCOUNT; i++)
            {
                var level = abffio.GetEpochLevel(0, 0, i);
                var dur = abffio.GetEpochDuration(0, 0, i);
                (var valid, var limit1, var limit2) = abffio.GetEpochLimits(0, 0, i);
                if (valid || i<0)
                    Debug.WriteLine($"Epoch {i}: level={level}, duration={dur}, from={limit1}, to={limit2}");
            }
            Debug.WriteLine($"pre-epoch holding: {abffio.GetHoldingLength()}");
        }

        public string GetDescription()
        {
            string info = "";

            // use reflection to iterate through all public variables of this class
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                // get the basic information for each item
                string type = field.FieldType.ToString();
                string name = field.Name;
                string value = field.GetValue(this).ToString();

                // customize string value of specific types
                if (field.FieldType == typeof(double))
                    value = string.Format("{0:0.0000}", (double)field.GetValue(this));
                else if (
                        field.FieldType == typeof(int) ||
                        field.FieldType == typeof(Int16) ||
                        field.FieldType == typeof(long)
                        )
                    value = string.Format("{0}", field.GetValue(this));
                else if (field.FieldType == typeof(string))
                    value = $"\"{field.GetValue(this)}\"";
                else if (field.FieldType == typeof(Tag[]))
                    foreach (Tag tag in (Tag[])field.GetValue(this))
                        value += "\n" + tag.ToString();
                else
                    name = $"({type}) {name}";

                if (value == "vsABF.Tag[]")
                    continue;

                info += $"{name}: {value}\n";
            }

            info = info.Trim();
            return info;
        }
    }
}
