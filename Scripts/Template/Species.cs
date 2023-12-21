using System;

[Serializable]
public class Species
{
    public string Name;
    public Lists.Type Type1;
    public Lists.Type Type2;
    public Lists.EggGroup EggGroup1;
    public Lists.EggGroup EggGroup2;
    public int[] BaseOffensive;
    public int[] BaseDefensive;
    public int[] BaseStatus;
}