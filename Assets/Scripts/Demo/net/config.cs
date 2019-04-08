using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace  CONFIG{
    [Serializable]
    public struct xy_Value{//xy参数
        public float x;
        public float y;
        
        public xy_Value(float x, float y){
            this.x = x;
            this.y = y;
        }
    }

    [Serializable]
     public struct Config_Value{
        public long num; //总人数
        public long[] ids;//所有玩家id
        public xy_Value[] values;//需配置的参数
    }   

    public struct XZ_Y{
        public Vector3 xz;
        public Quaternion y;
        public XZ_Y(Vector3 x, Quaternion y){
            this.xz = x;
            this.y = y;
        }
    }   
}


public class config{
    public static Dictionary<long, CONFIG.xy_Value> TankMap = new Dictionary<long, CONFIG.xy_Value>();    //存放玩家移动数据
    public static Dictionary<long, CONFIG.XZ_Y> XZ_Y_Map = new Dictionary<long, CONFIG.XZ_Y>();    //存放玩家位置数据
    public static Dictionary<String, long> id_Map = new Dictionary<String, long>();    //存放玩家ID

    public static Dictionary<long, String> name_Map = new Dictionary<long, String>();    //存放玩家名字


    public static void Add(long id){  //1. 添加玩家
        CONFIG.xy_Value xy =  new CONFIG.xy_Value();
        TankMap.Add(id, xy);
    }
    public static void Set_xy(long id, float x, float y){  //2. 根据id设置xy值
        if(TankMap.ContainsKey(id)){
            TankMap[id] = new CONFIG.xy_Value(x, y);
        }else{
            Debug.LogError("id不存在");
        } 
    }
    public static CONFIG.xy_Value Get_xy_by_id(long id){  //3. 根据id获取xy值
        return TankMap[id];
    }
    public static void Del(long id){    //4. 根据id删除玩家
        if(TankMap.ContainsKey(id)){
            TankMap.Remove(id);
        }
    }

//-------------------------------------------------
    public static void xyz_Add(long id){  //1. 添加玩家
        CONFIG.XZ_Y xyz =  new CONFIG.XZ_Y();
        XZ_Y_Map.Add(id, xyz);
    }
    public static void  xyz_Set(long id, Vector3 xz, Quaternion y){  //2. 根据id设置xy值
        if(XZ_Y_Map.ContainsKey(id)){
            XZ_Y_Map[id] = new CONFIG.XZ_Y(xz, y);
        }else{
            Debug.LogError("id不存在");
        } 
    }
    public static CONFIG.XZ_Y  xyz_Get_by_id(long id){  //3. 根据id获取xy值
        return XZ_Y_Map[id];
    }
    public static void  xyz_Del(long id){    //4. 根据id删除玩家
        if(XZ_Y_Map.ContainsKey(id)){
            XZ_Y_Map.Remove(id);
        }
    }
    //-------------------------------------------


    public static void ID_Add(String tag){  //1. 添加玩家
        id_Map.Add(tag, 0);
    }
    public static void Set_ID(String tag,long id){  //2. 根据id设置xy值
        if(id_Map.ContainsKey(tag)){
            id_Map[tag] = id;
        }else{
            Debug.LogError("id不存在");
        } 
    }
    public static long Get_id_by_Tag(String tag){  //3. 根据id获取xy值
        return id_Map[tag];
    }
    public static void Del(String tag){    //4. 根据id删除玩家
        if(id_Map.ContainsKey(tag)){
            id_Map.Remove(tag);
        }
    }

        //-------------------------------------------


    public static void Name_Add(long id){  //1. 添加玩家
        name_Map.Add(id, "");
    }
    public static void Set_Name(long id,string name){  //2. 根据id设置xy值
        if(name_Map.ContainsKey(id)){
            name_Map[id] = name;
        }else{
            Debug.LogError("id不存在");
        } 
    }
    public static string Get_Name_by_id(long id){  //3. 根据id获取xy值
        return name_Map[id];
    }
    public static void Name_Del(long id){    //4. 根据id删除玩家
        if(name_Map.ContainsKey(id)){
            name_Map.Remove(id);
        }
    }

}
