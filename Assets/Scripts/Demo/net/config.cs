using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  CONFIG{
    public struct xy_Value{//xy参数
        public float x;
        public float y;
        
        public xy_Value(float x, float y){
            this.x = x;
            this.y = y;
        }
    }

     public struct Config_Value{
        public long num; //总人数
        public long[] ids;//所有玩家id
        public xy_Value[] values;//需配置的参数
    }   
}


public class config{
    public static Dictionary<long, CONFIG.xy_Value> TankMap = new Dictionary<long, CONFIG.xy_Value>();    //存放玩家数据
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
}
