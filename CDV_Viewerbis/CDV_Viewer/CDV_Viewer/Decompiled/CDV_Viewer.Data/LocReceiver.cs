using System;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace CDV_Viewer.Data;

public class LocReceiver
{
	private static LocReceiver _currentLoc;

	private IPEndPoint _ip;

	private UdpClient _client;

	private bool _stop;

	private Timer _timer;

	private DateTime _lastTrame;

	private int _id;

	private int _deltaX;

	private int _ligne;

	private string _voie = string.Empty;

	private int _pk;

	private int _vitesse;

	private string _motrice;

	public static LocReceiver CurrentLoc
	{
		get
		{
			if (_currentLoc == null)
			{
				_currentLoc = new LocReceiver();
			}
			return _currentLoc;
		}
	}

	public int DeltaX => _deltaX;

	public int Ligne => _ligne;

	public string FormatLigne => Ligne.ToString("000000");

	public string Voie => _voie;

	public string FormatVoie
	{
		get
		{
			string text = Voie.ToString();
			while (text.Length < 6)
			{
				text = text.Insert(text.Length, " ");
			}
			return text;
		}
	}

	public int PK => _pk;

	public string FormatPK => (_pk / 1000).ToString("000") + "+" + (_pk % 1000).ToString("000");

	public int Vitesse => _vitesse;

	public string FormatVitesse => _vitesse + "km/h";

	public string Motrice => _motrice;

	public string FormatLoc => FormatLigne + " " + FormatVoie + " " + FormatPK;

	public event EventHandler LocReceived;

	public event EventHandler LocChanged;

	public event EventHandler MotriceChanged;

	public LocReceiver()
	{
		_timer = new Timer();
		_timer.Interval = 5000.0;
		_timer.Elapsed += Timer_Elapsed;
		_ip = new IPEndPoint(IPAddress.Any, Convert.ToInt32(50000));
		_client = new UdpClient(_ip);
	}

	public void Start()
	{
		_client.BeginReceive(OnReceive, _client);
	}

	public void Stop()
	{
		_stop = true;
	}

	private unsafe void ReadBuffer(byte[] buffer)
	{
		fixed (byte* ptr = &buffer[0])
		{
			byte[] array = new byte[4];
			_id += *ptr;
			_id += ptr[1];
			_id += ptr[2];
			array[0] = ptr[8];
			array[1] = ptr[9];
			array[2] = ptr[10];
			array[3] = ptr[11];
			_deltaX = BitConverter.ToInt32(array, 0);
			array[0] = ptr[14];
			array[1] = ptr[15];
			array[2] = ptr[16];
			array[3] = ptr[17];
			_ligne = BitConverter.ToInt32(array, 0);
			_voie = string.Empty;
			string voie = _voie;
			char c = (char)ptr[18];
			_voie = voie + c;
			string voie2 = _voie;
			c = (char)ptr[19];
			_voie = voie2 + c;
			string voie3 = _voie;
			c = (char)ptr[20];
			_voie = voie3 + c;
			string voie4 = _voie;
			c = (char)ptr[21];
			_voie = voie4 + c;
			string voie5 = _voie;
			c = (char)ptr[22];
			_voie = voie5 + c;
			string voie6 = _voie;
			c = (char)ptr[23];
			_voie = voie6 + c;
			_voie = _voie.Trim();
			array[0] = ptr[24];
			array[1] = ptr[25];
			array[2] = ptr[26];
			array[3] = ptr[27];
			_pk = BitConverter.ToInt32(array, 0);
			_vitesse = BitConverter.ToInt16(new byte[2]
			{
				ptr[29],
				ptr[30]
			}, 0);
			if (Convert.ToInt32(ptr[28]) == 161)
			{
				_motrice = "M1";
			}
			else
			{
				_motrice = "M2";
			}
		}
	}

	private void OnReceive(IAsyncResult ar)
	{
		if (!_stop)
		{
			_lastTrame = DateTime.Now;
			int deltaX = DeltaX;
			string motrice = Motrice;
			ReadBuffer(_client.Receive(ref _ip));
			if (this.LocReceived != null)
			{
				this.LocReceived(this, new EventArgs());
			}
			if (deltaX != DeltaX && this.LocChanged != null)
			{
				this.LocChanged(this, new EventArgs());
			}
			if (motrice != Motrice && this.MotriceChanged != null)
			{
				this.MotriceChanged(this, new EventArgs());
			}
			_client.BeginReceive(OnReceive, _client);
		}
	}

	private void Timer_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (DateTime.Now.Subtract(_lastTrame).TotalSeconds > 5.0)
		{
			_vitesse = 0;
		}
		if (this.LocChanged != null)
		{
			this.LocChanged(this, new EventArgs());
		}
	}
}
