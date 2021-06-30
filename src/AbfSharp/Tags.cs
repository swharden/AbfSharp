using System;
using System.Linq;

namespace AbfSharp
{
    public class Tags
    {
        private readonly Tag[] IndividualTags;

        /// <summary>
        /// Number of comments
        /// </summary>
        public int Count => IndividualTags.Length;

        /// <summary>
        /// Indicates what type each tag is
        /// </summary>
        public TagType[] Types => IndividualTags.Select(x => x.Type).ToArray();

        /// <summary>
        /// Time of each tag (seconds)
        /// </summary>
        public float[] Times => IndividualTags.Select(x => x.Time).ToArray();

        /// <summary>
        /// Time of each tag (minutes)
        /// </summary>
        public float[] TimesMin => IndividualTags.Select(x => x.TimeMin).ToArray();

        /// <summary>
        /// Comments (only populated for comment tags)
        /// </summary>
        public string[] Comments => IndividualTags.Select(x => x.Comment).ToArray();

        public Tags(ABFFIO.TagStruct[] tagStructs, float tagTimeMult)
        {
            IndividualTags = tagStructs.Select(x => new Tag(x, tagTimeMult)).ToArray();
        }

        public Tags(ABFFIO.TagStruct[] tagStructs, ABFFIO.AbfFileHeader header)
        {
            float sampleRate = 1e6f / header.fADCSequenceInterval / header.nADCNumChannels;
            float samplePeriod = 1.0f / sampleRate;
            float tagTimeMult = (header.fSynchTimeUnit == 0)
                ? samplePeriod / header.nADCNumChannels
                : header.fSynchTimeUnit / 1e6f;

            IndividualTags = tagStructs.Select(x => new Tag(x, tagTimeMult)).ToArray();
        }

        public override string ToString()
        {
            if (Count == 0)
                return "no tags";

            string[] tagSummaries = IndividualTags.Select(x => x.Summary).ToArray();
            return string.Join(", ", tagSummaries);
        }
    }
}
