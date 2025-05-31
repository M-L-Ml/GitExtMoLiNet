
using System.Diagnostics;
using System.Windows.Forms;
//used in GitUI.CommandsDialogs.BrowseDialog.DashboardControl

namespace System.Windows.Forms
{

    public enum ListViewGroupCollapsedState
    {
        Default,
        Expanded,
        Collapsed
    }


    public static class ListViewGroupEx//: ListViewGroup
    {
        //private string _text;
        //private HorizontalAlignment _left;

        //public ListViewGroup2(string text, HorizontalAlignment left)
        //{
        //    _text = text;
        //    _left = left;
        //}

        //public string Name { get; set; }
        public static void CollapsedState(this ListViewGroup t, ListViewGroupCollapsedState _) {
            Debug.Assert(false,"TODO implement");

        }
        public static void TaskLink(this ListViewGroup t, string _) { Debug.Assert(false,"TODO implement"); }
    }
}
