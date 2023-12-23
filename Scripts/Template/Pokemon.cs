using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Pokemon
{
    public string uuid;
    public string Name;
    public Species Species;
    public Lists.Ability Ability;
    public int[] Offensive;
    public int[] Defensive;
    public int[] Status;
    public List<Lists.Type> MajorBlood;
    public List<Lists.Type> MinorBlood;
    //public ArrayList FormAllele;
    public int[] ApexAllele;
    public int[] ShinyAllele;
    public string[] GrandParents;
    public string[] Parents;
}