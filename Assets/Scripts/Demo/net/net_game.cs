using UnityEngine;
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

    public enum MOVEOrder{MOVE_X,MOVE_Y}
    struct PlayerMsg {//玩家移动消息
        public long ID;
        public MOVEOrder order;
        public float cmd;//数值
    }

    struct GameStartMsg {//玩家移动消息
        public bool gamestart;//数值
    }

    public class SocketNetMgr{
        private string _ip;
        private int _port;
        private Socket _socket;

        private long _playerid;
        private long _roomid;
        private bool _rooming;
        private bool _gameing;


        //移动参数
        private bool _moveing_x;//x轴是否运动
        private bool _moveing_y;//y轴是否运动

        private CONFIG.Config_Value _gameinit; //游戏初始化位置配置


        public SocketNetMgr(){
            //this._ip = "193.112.143.141"; //改为自己对外的 IP
            this._ip = "127.0.0.1";
            this._port = 7000; //端口号
            this._playerid = 0;
            this._roomid = 0;
            this._rooming = false;
            this._gameing = false;
            this._gameinit.values = new CONFIG.xy_Value[4];
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
                Debug.LogError(data);
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

        public void Start(){//4. 开始游戏
            ClientMsg clientmsg;
            clientmsg.order = ClientOrder.START;
            this._socket.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(clientmsg)));
        }
        public void WaitGameStartMsg(){//5. 等待游戏开始指令
            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);
            GameStartMsg gamestartmsg = JsonUtility.FromJson<GameStartMsg>(data);
            if (gamestartmsg.gamestart == true){
                this._gameing = true;
                this.Start(); //给服务器器发送开始消息
            }
        }  
        public void GameMsgInit(){//6. 游戏初始化消息
            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);

            this._gameinit = JsonUtility.FromJson<CONFIG.Config_Value>(data);
        }  

        public void PlayerMove_X(float cmd){//5. X轴移动
            PlayerMsg playermsg;
            playermsg.order = MOVEOrder.MOVE_X;
            playermsg.ID = _playerid;
            playermsg.cmd = cmd;
            this._socket.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(playermsg)));

            /*
            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);
            PlayerMsg move = JsonUtility.FromJson<PlayerMsg>(data);

            CONFIG.xy_Value v = config.Get_xy_by_id(_playerid);
            config.Set_xy(_playerid, move.cmd, v.y);

            if(move.cmd != 0 )
                this._moveing_x = true;
            else
                this._moveing_x = false;
             */

        }

        public void PlayerMove_Y(float cmd){//6. Y轴移动
            PlayerMsg playermsg;
            playermsg.ID = _playerid;
            playermsg.order = MOVEOrder.MOVE_Y;
            playermsg.cmd = cmd;
            this._socket.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(playermsg)));
            
            /* 
            var buf = new byte[1024];
            int n = this._socket.Receive(buf);
            string data = Encoding.UTF8.GetString(buf,0,n);
            PlayerMsg move = JsonUtility.FromJson<PlayerMsg>(data);

            CONFIG.xy_Value v = config.Get_xy_by_id(_playerid);
            config.Set_xy(_playerid, v.x, move.cmd);

            if(move.cmd != 0 ) 
                this._moveing_y = true;
            else
                this._moveing_y = false;
            */
        }

        public void SetMoveValueByNet(){ //从服务器器获取运动状态并设置
            while(true){
                var buf = new byte[1024];
                int n = this._socket.Receive(buf);
                string data = Encoding.UTF8.GetString(buf,0,n);
                PlayerMsg move = JsonUtility.FromJson<PlayerMsg>(data);

                long p_id = move.ID;

                if(move.order == MOVEOrder.MOVE_X){
                    CONFIG.xy_Value v = config.Get_xy_by_id(p_id);
                    config.Set_xy(p_id, move.cmd, v.y);
                    if(move.cmd != 0 )
                        this._moveing_x = true;
                    else
                        this._moveing_x = false;

                }else if(move.order == MOVEOrder.MOVE_Y){
                    CONFIG.xy_Value v = config.Get_xy_by_id(p_id);
                    config.Set_xy(p_id, v.x, move.cmd);
                    if(move.cmd != 0 ) 
                        this._moveing_y = true;
                    else
                        this._moveing_y = false;
                }
            }    
        }

        public bool Get_moveing_x(){ //获取玩家X轴运动状态
            return this._moveing_x;
        }
        public bool Get_moveing_y(){//获取玩家Y轴运动状态
            return this._moveing_y;   
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
 
 
        public CONFIG.Config_Value GetGameinit(){  //获取游戏状态
            return this._gameinit; 
        }

        public string move(){   //*.
                var buf = new byte[1024];
                int n = this._socket.Receive(buf);
                string date = Encoding.UTF8.GetString(buf,0,n);
                return date;
        }

    }
}

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

    static public void Exit(){
        net.ExitRoom();
        Debug.Log("退出房间");
    }

    static public void Move_X(float cmd){//发送运动指令并改变运动状态
        net.PlayerMove_X(cmd);
    }

    static public void Move_Y(float cmd){//发送运动指令并改变运动状态
        net.PlayerMove_Y(cmd);
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
