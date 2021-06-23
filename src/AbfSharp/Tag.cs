using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public class Tag
    {
        public readonly long lTagTime;
        public readonly string sComment;
        public readonly short nTagType;
        public readonly short nVoiceTagNumber;

        /// <summary>
        /// Tag time (seconds)
        /// </summary>
        public readonly float Time;

        /// <summary>
        /// Tag time (minutes)
        /// </summary>
        public float TimeMin => Time / 60.0f;

        /// <summary>
        /// Tag comment (or empty string if no comment)
        /// </summary>
        public string Comment => sComment.Trim();

        public Tag(long lTagTime, string sComment, short nTagType, short nVoiceTagNumber, float tagTimeMult)
        {
            this.lTagTime = lTagTime;
            this.sComment = sComment;
            this.nTagType = nTagType;
            this.nVoiceTagNumber = nVoiceTagNumber;
            Time = lTagTime * tagTimeMult;
        }
    }
}
