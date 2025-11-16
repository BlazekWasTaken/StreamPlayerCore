using System.ComponentModel;

namespace StreamPlayerCore.WinForms.Demo;

partial class DemoForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
        panel1 = new System.Windows.Forms.Panel();
        panel2 = new System.Windows.Forms.Panel();
        tbUrl1 = new System.Windows.Forms.TextBox();
        tbUrl2 = new System.Windows.Forms.TextBox();
        btnStart1 = new System.Windows.Forms.Button();
        btnStop1 = new System.Windows.Forms.Button();
        btnStop2 = new System.Windows.Forms.Button();
        btnStart2 = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Location = new System.Drawing.Point(12, 12);
        panel1.Name = "panel1";
        panel1.Size = new System.Drawing.Size(640, 480);
        panel1.TabIndex = 0;
        // 
        // panel2
        // 
        panel2.Location = new System.Drawing.Point(662, 12);
        panel2.Name = "panel2";
        panel2.Size = new System.Drawing.Size(640, 480);
        panel2.TabIndex = 1;
        // 
        // tbUrl1
        // 
        tbUrl1.Location = new System.Drawing.Point(12, 498);
        tbUrl1.Name = "tbUrl1";
        tbUrl1.Size = new System.Drawing.Size(640, 23);
        tbUrl1.TabIndex = 2;
        // 
        // tbUrl2
        // 
        tbUrl2.Location = new System.Drawing.Point(662, 498);
        tbUrl2.Name = "tbUrl2";
        tbUrl2.Size = new System.Drawing.Size(640, 23);
        tbUrl2.TabIndex = 3;
        // 
        // btnStart1
        // 
        btnStart1.Location = new System.Drawing.Point(12, 527);
        btnStart1.Name = "btnStart1";
        btnStart1.Size = new System.Drawing.Size(112, 30);
        btnStart1.TabIndex = 4;
        btnStart1.Text = "Start";
        btnStart1.UseVisualStyleBackColor = true;
        btnStart1.Click += btnStart1_Click;
        // 
        // btnStop1
        // 
        btnStop1.Location = new System.Drawing.Point(130, 527);
        btnStop1.Name = "btnStop1";
        btnStop1.Size = new System.Drawing.Size(112, 30);
        btnStop1.TabIndex = 5;
        btnStop1.Text = "Stop";
        btnStop1.UseVisualStyleBackColor = true;
        btnStop1.Click += btnStop1_Click;
        // 
        // btnStop2
        // 
        btnStop2.Location = new System.Drawing.Point(780, 527);
        btnStop2.Name = "btnStop2";
        btnStop2.Size = new System.Drawing.Size(112, 30);
        btnStop2.TabIndex = 7;
        btnStop2.Text = "Stop";
        btnStop2.UseVisualStyleBackColor = true;
        btnStop2.Click += btnStop2_Click;
        // 
        // btnStart2
        // 
        btnStart2.Location = new System.Drawing.Point(662, 527);
        btnStart2.Name = "btnStart2";
        btnStart2.Size = new System.Drawing.Size(112, 30);
        btnStart2.TabIndex = 6;
        btnStart2.Text = "Start";
        btnStart2.UseVisualStyleBackColor = true;
        btnStart2.Click += btnStart2_Click;
        // 
        // DemoForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1314, 571);
        Controls.Add(btnStop2);
        Controls.Add(btnStart2);
        Controls.Add(btnStop1);
        Controls.Add(btnStart1);
        Controls.Add(tbUrl2);
        Controls.Add(tbUrl1);
        Controls.Add(panel2);
        Controls.Add(panel1);
        Text = "DemoForm";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.TextBox tbUrl1;
    private System.Windows.Forms.TextBox tbUrl2;
    private System.Windows.Forms.Button btnStart1;
    private System.Windows.Forms.Button btnStop1;
    private System.Windows.Forms.Button btnStop2;
    private System.Windows.Forms.Button btnStart2;

    private System.Windows.Forms.Panel panel2;

    private System.Windows.Forms.Panel panel1;

    #endregion
}