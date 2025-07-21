using System.Collections;
using System.Runtime.ExceptionServices;
using System.Text;

namespace GitCommands
{
    public static class ExceptionUtils
    {
        public static void ShowException(Exception e, bool canIgnore = true)
        {
            ShowException(e, string.Empty, canIgnore);
        }

        public static void ShowException(Exception e, string info, bool canIgnore = true)
        {
            ShowException(null, e, info, canIgnore);
        }

        public static void ShowException(IWin32Window? owner, Exception e, string info, bool canIgnore)
        {
            if (!(canIgnore && IsIgnorable(e)))
            {
                MessageBox.Show(owner, string.Join(Environment.NewLine + Environment.NewLine, info, e.ToStringWithData()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool IsIgnorable(Exception e)
        {
            return e is ThreadAbortException;
        }

        public static string ToStringWithData(this Exception e)
        {
            StringBuilder sb = new();
            sb.AppendLine(e.ToString());
            sb.AppendLine();
            foreach (DictionaryEntry entry in e.Data)
            {
                sb.AppendLine(entry.Key + " = " + entry.Value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// ExceptionDispatchInfo.Capture
        /// </summary>
        /// <param name="e"></param>
        /// <returns>same input exception</returns>
        public static Exception PreserveStackDetails(this Exception e)
        {
            ExceptionDispatchInfo.Capture(e);
            if (e is AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    inner.PreserveStackDetails();
                }
            }
            else if (e is System.Reflection.TargetInvocationException tie)
            {
                tie.InnerException?.PreserveStackDetails();
            }

            return e;
        }
    }
}
