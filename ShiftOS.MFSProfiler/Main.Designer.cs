/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace ShiftOS.MFSProfiler
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvfiles = new System.Windows.Forms.TreeView();
            this.pnlfileinfo = new System.Windows.Forms.Panel();
            this.pnldirectorylisting = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtbinary = new System.Windows.Forms.TextBox();
            this.txtascii = new System.Windows.Forms.TextBox();
            this.lbfileinfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlfileinfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvfiles);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlfileinfo);
            this.splitContainer1.Panel2.Controls.Add(this.pnldirectorylisting);
            this.splitContainer1.Size = new System.Drawing.Size(739, 466);
            this.splitContainer1.SplitterDistance = 246;
            this.splitContainer1.TabIndex = 0;
            // 
            // tvfiles
            // 
            this.tvfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvfiles.Location = new System.Drawing.Point(0, 30);
            this.tvfiles.Name = "tvfiles";
            this.tvfiles.Size = new System.Drawing.Size(246, 436);
            this.tvfiles.TabIndex = 0;
            this.tvfiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvfiles_AfterSelect);
            // 
            // pnlfileinfo
            // 
            this.pnlfileinfo.Controls.Add(this.groupBox2);
            this.pnlfileinfo.Controls.Add(this.groupBox1);
            this.pnlfileinfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlfileinfo.Location = new System.Drawing.Point(0, 0);
            this.pnlfileinfo.Name = "pnlfileinfo";
            this.pnlfileinfo.Size = new System.Drawing.Size(489, 466);
            this.pnlfileinfo.TabIndex = 0;
            // 
            // pnldirectorylisting
            // 
            this.pnldirectorylisting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnldirectorylisting.Location = new System.Drawing.Point(0, 0);
            this.pnldirectorylisting.Name = "pnldirectorylisting";
            this.pnldirectorylisting.Size = new System.Drawing.Size(489, 466);
            this.pnldirectorylisting.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(246, 30);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Mount file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbfileinfo);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(489, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File record information";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tabControl1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 164);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(489, 302);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Contents";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(483, 283);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtascii);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(475, 257);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ASCII";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtbinary);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(475, 257);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Binary";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtbinary
            // 
            this.txtbinary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtbinary.Location = new System.Drawing.Point(3, 3);
            this.txtbinary.Multiline = true;
            this.txtbinary.Name = "txtbinary";
            this.txtbinary.Size = new System.Drawing.Size(469, 251);
            this.txtbinary.TabIndex = 0;
            // 
            // txtascii
            // 
            this.txtascii.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtascii.Location = new System.Drawing.Point(3, 3);
            this.txtascii.Multiline = true;
            this.txtascii.Name = "txtascii";
            this.txtascii.Size = new System.Drawing.Size(469, 251);
            this.txtascii.TabIndex = 0;
            // 
            // lbfileinfo
            // 
            this.lbfileinfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbfileinfo.Location = new System.Drawing.Point(3, 16);
            this.lbfileinfo.Name = "lbfileinfo";
            this.lbfileinfo.Size = new System.Drawing.Size(483, 145);
            this.lbfileinfo.TabIndex = 0;
            this.lbfileinfo.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 466);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlfileinfo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvfiles;
        private System.Windows.Forms.Panel pnldirectorylisting;
        private System.Windows.Forms.Panel pnlfileinfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtascii;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtbinary;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbfileinfo;
    }
}

