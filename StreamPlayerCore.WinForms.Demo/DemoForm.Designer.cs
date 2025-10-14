namespace StreamPlayerCore.WinForms.Demo;

partial class DemoForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        streamPlayerControl1 = new StreamPlayerCore.WinForms.Control.StreamPlayerControl();
        streamPlayerControl2 = new StreamPlayerCore.WinForms.Control.StreamPlayerControl();
        button1 = new System.Windows.Forms.Button();
        button2 = new System.Windows.Forms.Button();
        textBox1 = new System.Windows.Forms.TextBox();
        textBox2 = new System.Windows.Forms.TextBox();
        textBox3 = new System.Windows.Forms.TextBox();
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        SuspendLayout();
        // 
        // streamPlayerControl1
        // 
        streamPlayerControl1.Location = new System.Drawing.Point(13, 11);
        streamPlayerControl1.Name = "streamPlayerControl1";
        streamPlayerControl1.Size = new System.Drawing.Size(783, 399);
        streamPlayerControl1.TabIndex = 0;
        // 
        // streamPlayerControl2
        // 
        streamPlayerControl2.Location = new System.Drawing.Point(12, 12);
        streamPlayerControl2.Name = "streamPlayerControl2";
        streamPlayerControl2.Size = new System.Drawing.Size(760, 472);
        streamPlayerControl2.TabIndex = 1;
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(672, 519);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(100, 30);
        button1.TabIndex = 2;
        button1.Text = "Play";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // button2
        // 
        button2.Location = new System.Drawing.Point(566, 519);
        button2.Name = "button2";
        button2.Size = new System.Drawing.Size(100, 30);
        button2.TabIndex = 4;
        button2.Text = "Stop";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // textBox1
        // 
        textBox1.Location = new System.Drawing.Point(12, 524);
        textBox1.Name = "textBox1";
        textBox1.Size = new System.Drawing.Size(548, 23);
        textBox1.TabIndex = 3;
        // 
        // textBox2
        // 
        textBox2.Location = new System.Drawing.Point(672, 490);
        textBox2.Name = "textBox2";
        textBox2.Size = new System.Drawing.Size(100, 23);
        textBox2.TabIndex = 5;
        textBox2.Text = "5000000";
        // 
        // textBox3
        // 
        textBox3.Location = new System.Drawing.Point(460, 490);
        textBox3.Name = "textBox3";
        textBox3.Size = new System.Drawing.Size(100, 23);
        textBox3.TabIndex = 6;
        textBox3.Text = "0";
        // 
        // label1
        // 
        label1.Location = new System.Drawing.Point(566, 490);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(100, 23);
        label1.TabIndex = 7;
        label1.Text = "probe size";
        label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // label2
        // 
        label2.Location = new System.Drawing.Point(354, 490);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(100, 23);
        label2.TabIndex = 8;
        label2.Text = "analyze duration";
        label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // DemoForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(784, 561);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(textBox3);
        Controls.Add(textBox2);
        Controls.Add(button2);
        Controls.Add(textBox1);
        Controls.Add(button1);
        Controls.Add(streamPlayerControl2);
        Controls.Add(streamPlayerControl1);
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Label label1;

    private System.Windows.Forms.Button button2;

    private StreamPlayerCore.WinForms.Control.StreamPlayerControl streamPlayerControl2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;

    private StreamPlayerCore.WinForms.Control.StreamPlayerControl streamPlayerControl1;

    #endregion
}