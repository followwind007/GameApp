using System.Collections.Generic;

namespace GameApp.DataBinder
{
    public interface IBindableTarget
    {
        BindableValues Vals { get; }

        string LuaPath { get; }
        List<LuaPath> Interfaces { get; }

        HashSet<string> Methods { get; set; }
        
        bool InitDone { get; set; }
    }
}