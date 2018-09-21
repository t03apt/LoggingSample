using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LoggingSample
{
    [Serializable]
    public sealed class LogHeader : Dictionary<string, object>
    {
        public LogHeader()
        {
        }

        private LogHeader(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            bool initial = true;

            foreach (KeyValuePair<string, object> next in this)
            {
                if (!initial)
                {
                    builder.Append("; ");
                }

                builder.Append(next.Key);
                builder.Append(" = ");
                builder.Append(next.Value);
                initial = false;
            }

            return builder.ToString();
        }
    }
}