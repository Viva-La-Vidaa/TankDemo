using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Threading; 


namespace GAME{
    //ClientMsg 客户端消息
    public enum ClientOrder{JOIN,EXIT,START}
    struct ClientMsg {
        public ClientOrder order;//加入或者退出房间 
    }

    //RobbyMsg 大厅消息
    struct RobbyMsg  {
        public bool joinlobby;//从服务器接收到的消息，表示是否成功加入大厅
        public long playerid;//分配的玩家id

    }

    //RoomMsg 房间消息
    public class RoomMsg  {
        public bool joinroom;//表示是否成功加入房间
        public long roomid;//加入的房间id
    }
    public enum MOVEOrder{MOVE_X,MOVE_Y,SITE}
    public struct PlayerMsg {//玩家操作消息
        public long ID;
        public MOVEOrder order;
        public int q;//数值
        public int v;//数值

        public int num;

        public Vector3 V;

        public Quaternion Q;
    }

    struct GameStartMsg {//玩家移动消息
        public bool gamestart;//数值
    }

    struct MsgHead {//消息头
        public int size;//消息长度
    }

   
    public class SocketNetMgr{
        private string _ip;
        private int _port;
        private Socket _socket;

        private long _playerid;
        private long _roomid;
        private bool _rooming;
        private bool _gameing;
        private bool _startinit;


        private CONFIG.Config_Value _gameinit; //游戏初始化位置配置

        public Queue<PlayerMsg> FrameList;//帧同步队列

        public Text t;
        int My = -1;
        int Server_id = 0;
        int Cline_id = 0;
        int X = 0;
        int Y = 0;

        int buzhen = 0;


        public SocketNetMgr(){
            //this._ip = "193.112.143.141"; //改为自己对外的 IP
            this._ip = "127.0.0.1";
            this._port = 7000; //端口号
            this._playerid = 0;
            this._roomid = 0;
            this._rooming = false;
            this._gameing = false;
            this._gameinit.values = new CONFIG.xy_Value[4];
            this.FrameList = new Queue<PlayerMsg>();
            t = GameObject.FindGameObjectWithTag("T").GetComponent<Text>();
            if(t== null){
                Debug.LogError("--");
            }
            t.text = " ";
        }

        public byte[] pack(int size, byte[] body){//打包
            MsgHead msghead;
            msghead.size = size;
            byte[] head = Encoding.UTF8.GetBytes(JsonUtility.ToJson(msghead));
            byte[] tmp = new byte[head.Length + body.Length];
            System.Buffer.BlockCopy(head, 0, tmp, 0, head.Length);
            System.Buffer.BlockCopy(body, 0, tmp, head.Length, body.Length);
            return tmp;
        }

            //连接服务器
        public bool Connect(){//1. 连接
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.Connect(this._ip, this._port);
            if(!this._socket.Connected){
                return false;
            }

            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);
            RobbyMsg lobbymsg = JsonUtility.FromJson<RobbyMsg>(data);
            this._playerid = lobbymsg.playerid;
            return lobbymsg.joinlobby;
        }
        public bool JoinRoom(){//2. 加入房间
            if(!_rooming){
                ClientMsg clientmsg;
                clientmsg.order = ClientOrder.JOIN;
                this._socket.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(clientmsg)));

                var buf = new byte[1024];
                int n = this._socket.Receive(buf);
                string data = Encoding.UTF8.GetString(buf,0,n);
                Debug.LogWarning(data);
                RoomMsg roomsg = JsonUtility.FromJson<RoomMsg>(data);
                this._roomid = roomsg.roomid;
                this._rooming = true;
                return roomsg.joinroom;
            }
            return false;
        }
        public void ExitRoom(){//3. 返回大厅
            ClientMsg clientmsg;
            clientmsg.order = ClientOrder.EXIT;
            this._socket.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(clientmsg)));
            this._rooming = false;
        }

        public void SendStartMsg(){//4. 开始游戏
            ClientMsg clientmsg;
            clientmsg.order = ClientOrder.START;
            this._socket.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(clientmsg)));
            this._startinit = true;//可以开始初始化操作
        }
        public void WaitGameStartMsg(){//5. 等待游戏开始指令
            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);
            GameStartMsg gamestartmsg = JsonUtility.FromJson<GameStartMsg>(data);

            if (gamestartmsg.gamestart == true){
                this.SendStartMsg(); //给服务器器发送开始消息
            }
        }  

        public void GameMsgInit(){//6. 游戏初始化消息
            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);

            this._gameinit = JsonUtility.FromJson<CONFIG.Config_Value>(data);
        }  
        public void GameStart(){//7. 游戏初始化消息
            this._gameing = true;
        } 


        public void PlayerMove(float q, float v, int num){//8.移动
            PlayerMsg playermsg;
            playermsg.ID = _playerid;
            playermsg.order = MOVEOrder.MOVE_Y;
            playermsg.q = (int)q;
            playermsg.v = (int)v;
            playermsg.num = num;
            playermsg.V = Vector3.zero;
            playermsg.Q = new Quaternion();
            byte[] body = Encoding.UTF8.GetBytes(JsonUtility.ToJson(playermsg));
            byte[] data = pack(body.Length, body);
            this._socket.Send(data);
        }
        
        public void SetMoveValueByNet(){ //9 从服务器器获取运动状态并设置
            while(this._gameing){//正在游戏中
                //读包头
                var buf = new byte[1024];
                int n1 = this._socket.Receive(buf, 12, 0);
                if(n1 != 12){
                    Debug.LogError("消息长度出错");
                    continue;
                }
                string headmsg = Encoding.UTF8.GetString(buf,0,n1);
                //Debug.LogWarning(headmsg);
                MsgHead head = JsonUtility.FromJson<MsgHead>(headmsg);
 
                int n2 = this._socket.Receive(buf, head.size, 0);
                if(n2 != head.size){
                    Debug.LogError("消息长度出错");
                    continue;
                }
                string data = Encoding.UTF8.GetString(buf,0,n2);
                //Debug.LogWarning(data);

                PlayerMsg move = JsonUtility.FromJson<PlayerMsg>(data);
                FrameList.Enqueue(move);
            }    
        }        

        public void Operate(){ //10. 进行移动操作
            //1. 修正数值
            t.text =" ";
            
            if(FrameList.Count != 0){
                Server_id++;
                X = My - Cline_id;// X表示上次状态运行了几帧
                int flag = Cline_id;
                Cline_id = My;//Cline_i 表示客户端在第几帧帧运动状态改变的
                PlayerMsg move = FrameList.Dequeue();
                Y = move.num;
                int Z = Y - X;
                if(Z != 0){//补帧
                    buzhen++;
                    foreach (long ID in config.name_Map.Keys)
                    {
                        GameObject player = GameObject.Find(config.Get_Name_by_id(ID));
                        if(player == null){
                            Debug.LogError("this.GameObject错误");
                            return;
                        }

                        //获取移动数值
                        float _Q = config.Get_xy_by_id(ID).x ;
                        float _V = config.Get_xy_by_id(ID).y ;

                        if(_Q == 0 && _V == 0)
                            continue;

                        Quaternion old_Q = player.transform.rotation;
                        Vector3 oldpos = player.transform.position;

                        if(Z < 0){
                            _Q = -_Q;
                            _V = -_V;
                            Z = -Z;

                            //移动
                            if(_V != 0){
                                Vector3 tarPos = oldpos + player.transform.forward  * _V;
                                player.transform.position  = Vector3.Lerp(oldpos, tarPos, Time.deltaTime * Z * 5.0f);
                            }

                            //旋转 
                            if(_Q != 0){
                                Quaternion targetAngels =  Quaternion.Euler(0, player.transform.localEulerAngles.y + _Q * 90, 0);
                                player.transform.rotation = Quaternion.Slerp(old_Q, targetAngels, Time.deltaTime * Z);
                            }
                        }else{
                            //旋转 
                            if(_Q != 0){
                                Quaternion targetAngels =  Quaternion.Euler(0, player.transform.localEulerAngles.y + _Q * 90, 0);
                                player.transform.rotation = Quaternion.Slerp(old_Q, targetAngels, Time.deltaTime * Z);
                            }
                            //移动
                            if(_V != 0){
                                Vector3 tarPos = oldpos + player.transform.forward  * _V;
                                player.transform.position  = Vector3.Lerp(oldpos, tarPos, Time.deltaTime * Z * 5.0f);
                            }
                        }

                        Debug.LogError("补帧: "+ "Z: " + Z + " _V: " + _V + " _Q: " + _Q);
                        Debug.LogError("信息1: "+ "服务器当前帧数：" + Move.my + " 客户端当前帧数: " + My);
                        Debug.LogError("信息2: "+ "服务器改变状态帧数：" + Move.F + " 客户端改变状态帧数: " + Cline_id + "服务器上次：" + Move.o  + " 客户端上次: " + flag);
                        Debug.LogError("信息3: "+ "服务器状态维持帧数：" + move.num + " 客户端状态维持帧数: " + X);
                        Debug.LogError(" old_V: " + oldpos.x +" "+ oldpos.y+" "+ oldpos.z);
                        Debug.LogError(" new_V: " + player.transform.position.x +" "+ player.transform.position.y+" "+ player.transform.position.z);
                        Debug.LogError(" old_Q: " + old_Q.x +" "+ old_Q.y+" "+ old_Q.z+" "+ old_Q.w);
                        Debug.LogError(" new_Q: " + player.transform.rotation.x +" "+ player.transform.rotation.y+" "+ player.transform.rotation.z+" "+ player.transform.rotation.w);
 
                    }
                }

                if(move.order == MOVEOrder.SITE)
                    config.xyz_Set(move.ID,move.V,move.Q);
                else {
                    config.Set_xy(move.ID, move.q, move.v);
                }

            }

            if(this._gameing){//初始化已完成，正在游戏中
                //2. 进行移动
                foreach (long ID in config.name_Map.Keys)
                {
                    GameObject player = GameObject.Find(config.Get_Name_by_id(ID));
                    if(player == null){
                        Debug.LogError("this.GameObject错误");
                        return ;
                    }

                    //获取移动数值
                    float _Q = config.Get_xy_by_id(ID).x;
                    float _V = config.Get_xy_by_id(ID).y; 
                    t.text += "\n" + "移动数值修改:  " + "_Q: "+ _Q + "  " + "_V: "+_V;

                    //旋转 
                    if(_Q != 0){
                        Quaternion targetAngels =  Quaternion.Euler(0, player.transform.localEulerAngles.y + _Q * 90.0F, 0);
                        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetAngels, Time.deltaTime  );
                    }

                    //移动
                    if(_V != 0){
                        Vector3 oldpos = player.transform.position;
                        Vector3 tarPos = oldpos + player.transform.forward  * _V;
                        player.transform.position  = Vector3.Lerp(oldpos, tarPos, Time.deltaTime * 5.0f);
                   }
                    t.text += "\n"+ID + ": " + player.transform.position.x+" , "+ player.transform.position.y+" , "+ player.transform.position.z ;
                    t.text +="---"+ player.transform.rotation.x+" , "+ player.transform.rotation.y+" , "+ player.transform.rotation.z +" , "+ player.transform.rotation.w; 
                }

                t.text += "\n"+"当前是服务器第 " + Server_id + " 帧 " + " 客户端更新状态在第 " + Cline_id + " 帧 ";

                t.text += "\n"+"客户端上次状态维持了 " + X + " 帧 " + " 服务器上次状态维持了 " + Y + " 帧";
                t.text += "\n"+"补帧次数 " + buzhen;
                My++;
            }

        }      

        public long GetPlayerId(){ //获取玩家id
            return this._playerid; //连接大厅之后才有效
        }
        public long GetRoomId(){  //获取房间id
            return this._roomid;  //加入房间之后才有效
        }

        public bool GetRooming(){  //获取房间状态
            return this._rooming; 
        }

        public bool GetGameing(){  //获取游戏状态
            return this._gameing; 
        }

        public bool GetStart(){  //获取开始状态
            return this._startinit; 
        }
 
        public CONFIG.Config_Value GetGameinit(){  //获取初始化地图
            return this._gameinit; 
        }

    }
}

//-----------------------------------------
public class net_game : MonoBehaviour
{
    static public GAME.SocketNetMgr net;
    void Start()
    {
        //测试连接大厅
        net = new GAME.SocketNetMgr();//创建网络对象
        bool istrue0 = net.Connect();//连接大厅 
        if (istrue0) {
            Debug.Log("进入大厅");
            //测试
            Game_Text();
        } else{
            Debug.LogError("进入大厅失败");
        }
    }
    
    static public void Join(){
        bool istrue1 = net.JoinRoom(); //加入房间
        if(istrue1){
            Debug.Log(net.GetPlayerId());//获取玩家id
            Debug.Log(net.GetRoomId());//获取房间id
            Debug.Log("成功加入房间");
        }else{
            Debug.Log("加入失败，或已在房间");
        }
    }

    static public void Exit(){//退出房间
        net.ExitRoom();
        Debug.Log("退出房间");
    }

    static public void Move(float q, float v, int num){//发送运动指令并改变运动状态
        net.PlayerMove(q, v, num);
    }
    
    static public void Game_Text(){
        Join();//方便加入房间，后续要改动
        if(net.GetRooming()){
            Debug.Log("等待游戏开始");
            Thread thread = new Thread(new ThreadStart(net.WaitGameStartMsg));
            thread.Start();
        }
    }

}
