using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class StructureDef
{
    public string name; 


    public List<StructureTile> tiles;

}



[Serializable]
public class StructureTile
{
    public string tilename;

    public string Layer; 

    public int x; 

    public int y;
}
