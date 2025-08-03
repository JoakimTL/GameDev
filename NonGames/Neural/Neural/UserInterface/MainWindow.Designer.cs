namespace Neural.UserInterface;

partial class MainWindow {
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose( bool disposing ) {
		if (disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose( disposing );
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
		this.components = new System.ComponentModel.Container();
		this.txtIn = new RichTextBox();
		this.label1 = new Label();
		this.txtOut = new RichTextBox();
		this.label3 = new Label();
		this.globalTimer = new System.Windows.Forms.Timer( this.components );
		this.txtDebug = new RichTextBox();
		this.txtCharDebug = new RichTextBox();
		this.txtRepeatMessage = new TextBox();
		this.txtInterval = new TextBox();
		this.txtDebug2 = new RichTextBox();
		this.txtCharDebug2 = new RichTextBox();
		SuspendLayout();
		// 
		// txtIn
		// 
		this.txtIn.Location = new Point( 12, 27 );
		this.txtIn.Name = "txtIn";
		this.txtIn.Size = new Size( 300, 200 );
		this.txtIn.TabIndex = 0;
		this.txtIn.Text = "";
		this.txtIn.KeyUp += txtIn_KeyUp;
		// 
		// label1
		// 
		this.label1.AutoSize = true;
		this.label1.Location = new Point( 12, 9 );
		this.label1.Name = "label1";
		this.label1.Size = new Size( 35, 15 );
		this.label1.TabIndex = 1;
		this.label1.Text = "Input";
		// 
		// txtOut
		// 
		this.txtOut.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		this.txtOut.Location = new Point( 930, 27 );
		this.txtOut.Name = "txtOut";
		this.txtOut.ReadOnly = true;
		this.txtOut.Size = new Size( 300, 411 );
		this.txtOut.TabIndex = 3;
		this.txtOut.Text = "";
		// 
		// label3
		// 
		this.label3.AutoSize = true;
		this.label3.Location = new Point( 930, 9 );
		this.label3.Name = "label3";
		this.label3.Size = new Size( 45, 15 );
		this.label3.TabIndex = 5;
		this.label3.Text = "Output";
		// 
		// globalTimer
		// 
		this.globalTimer.Enabled = true;
		this.globalTimer.Interval = 20;
		this.globalTimer.Tick += globalTimer_Tick;
		// 
		// txtDebug
		// 
		this.txtDebug.Location = new Point( 318, 27 );
		this.txtDebug.Name = "txtDebug";
		this.txtDebug.ReadOnly = true;
		this.txtDebug.Size = new Size( 300, 200 );
		this.txtDebug.TabIndex = 6;
		this.txtDebug.Text = "";
		// 
		// txtCharDebug
		// 
		this.txtCharDebug.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		this.txtCharDebug.Location = new Point( 318, 233 );
		this.txtCharDebug.Name = "txtCharDebug";
		this.txtCharDebug.ReadOnly = true;
		this.txtCharDebug.Size = new Size( 300, 205 );
		this.txtCharDebug.TabIndex = 7;
		this.txtCharDebug.Text = "";
		// 
		// txtRepeatMessage
		// 
		this.txtRepeatMessage.Location = new Point( 12, 233 );
		this.txtRepeatMessage.Name = "txtRepeatMessage";
		this.txtRepeatMessage.Size = new Size( 300, 23 );
		this.txtRepeatMessage.TabIndex = 8;
		// 
		// txtInterval
		// 
		this.txtInterval.Location = new Point( 12, 262 );
		this.txtInterval.Name = "txtInterval";
		this.txtInterval.Size = new Size( 73, 23 );
		this.txtInterval.TabIndex = 9;
		// 
		// txtDebug2
		// 
		this.txtDebug2.Location = new Point( 624, 27 );
		this.txtDebug2.Name = "txtDebug2";
		this.txtDebug2.ReadOnly = true;
		this.txtDebug2.Size = new Size( 300, 200 );
		this.txtDebug2.TabIndex = 10;
		this.txtDebug2.Text = "";
		// 
		// txtCharDebug2
		// 
		this.txtCharDebug2.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		this.txtCharDebug2.Location = new Point( 624, 233 );
		this.txtCharDebug2.Name = "txtCharDebug2";
		this.txtCharDebug2.ReadOnly = true;
		this.txtCharDebug2.Size = new Size( 300, 205 );
		this.txtCharDebug2.TabIndex = 11;
		this.txtCharDebug2.Text = "";
		// 
		// MainWindow
		// 
		this.AutoScaleDimensions = new SizeF( 7F, 15F );
		this.AutoScaleMode = AutoScaleMode.Font;
		this.ClientSize = new Size( 1245, 450 );
		this.Controls.Add( this.txtCharDebug2 );
		this.Controls.Add( this.txtDebug2 );
		this.Controls.Add( this.txtInterval );
		this.Controls.Add( this.txtRepeatMessage );
		this.Controls.Add( this.txtCharDebug );
		this.Controls.Add( this.txtDebug );
		this.Controls.Add( this.label3 );
		this.Controls.Add( this.txtOut );
		this.Controls.Add( this.label1 );
		this.Controls.Add( this.txtIn );
		this.Name = "MainWindow";
		this.Text = "MainWindow";
		ResumeLayout( false );
		PerformLayout();
	}

	#endregion

	private RichTextBox txtIn;
	private Label label1;
	private RichTextBox txtOut;
	private Label label3;
	private System.Windows.Forms.Timer globalTimer;
	private RichTextBox txtDebug;
	private RichTextBox txtCharDebug;
	private TextBox txtRepeatMessage;
	private TextBox txtInterval;
	private RichTextBox txtDebug2;
	private RichTextBox txtCharDebug2;
}