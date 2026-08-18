using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class SelectJointPrincipauxPopupForm : PopupForm
{
	private DataGridView dgvJoints;

	public List<SIGJoint> Joints { get; }

	public SIGJoint JointDebut { get; private set; }

	public SIGJoint JointFin { get; private set; }

	public SelectJointPrincipauxPopupForm(List<SIGJoint> joints)
	{
		InitializeComponent();
		Joints = joints;
		foreach (SIGJoint joint in Joints)
		{
			DataGridViewRow dataGridViewRow = new DataGridViewRow
			{
				Tag = joint
			};
			dataGridViewRow.Cells.AddRange(new DataGridViewTextBoxCell
			{
				Value = string.Empty
			}, new DataGridViewTextBoxCell
			{
				Value = $"{joint.Type} sur {joint.Voie.Ligne.ID}:{joint.Voie.Nom} Pk {Chaines.PkToString(joint.PK)} ({joint.ID})"
			});
			dgvJoints.Rows.Add(dataGridViewRow);
		}
		if (joints.Count == 2)
		{
			SelectJoint(0);
			SelectJoint(1);
		}
		dgvJoints.CellClick += delegate(object s, DataGridViewCellEventArgs e)
		{
			SelectJoint(e.RowIndex);
		};
		dgvJoints.CellDoubleClick += delegate(object s, DataGridViewCellEventArgs e)
		{
			SelectJoint(e.RowIndex);
		};
	}

	public SelectJointPrincipauxPopupForm(List<SIGDemiJoint> demiJoints)
	{
		InitializeComponent();
		Joints = new List<SIGJoint>();
		foreach (SIGDemiJoint demiJoint in demiJoints)
		{
			SIGJoint joint = demiJoint.Joint;
			Joints.Add(joint);
			DataGridViewRow dataGridViewRow = new DataGridViewRow
			{
				Tag = joint
			};
			dataGridViewRow.Cells.AddRange(new DataGridViewTextBoxCell
			{
				Value = string.Empty
			}, new DataGridViewTextBoxCell
			{
				Value = $"{joint.Type} sur {joint.Voie.Ligne.ID}:{joint.Voie.Nom} Pk {Chaines.PkToString(joint.PK)} ({joint.ID})"
			});
			dgvJoints.Rows.Add(dataGridViewRow);
		}
		int num = 0;
		foreach (SIGDemiJoint demiJoint2 in demiJoints)
		{
			if (demiJoint2.Principal)
			{
				SelectJoint(num++);
			}
		}
		dgvJoints.CellClick += delegate(object s, DataGridViewCellEventArgs e)
		{
			SelectJoint(e.RowIndex);
		};
		dgvJoints.CellDoubleClick += delegate(object s, DataGridViewCellEventArgs e)
		{
			SelectJoint(e.RowIndex);
		};
	}

	private void InitializeComponent()
	{
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		this.dgvJoints = new System.Windows.Forms.DataGridView();
		((System.ComponentModel.ISupportInitialize)this.dgvJoints).BeginInit();
		base.SuspendLayout();
		this.dgvJoints.AllowUserToAddRows = false;
		this.dgvJoints.AllowUserToDeleteRows = false;
		this.dgvJoints.AllowUserToResizeRows = false;
		this.dgvJoints.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dgvJoints.BackgroundColor = System.Drawing.Color.White;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvJoints.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.dgvJoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvJoints.Columns.AddRange(new System.Windows.Forms.DataGridViewTextBoxColumn
		{
			HeaderText = "Principal",
			Name = "cPrincipal",
			ReadOnly = true,
			Width = 50
		}, new System.Windows.Forms.DataGridViewTextBoxColumn
		{
			HeaderText = "Joint",
			Name = "cJoint",
			ReadOnly = true,
			Width = 160
		});
		this.dgvJoints.GridColor = System.Drawing.SystemColors.Control;
		this.dgvJoints.Location = new System.Drawing.Point(14, 13);
		this.dgvJoints.Name = "dgvJoints";
		this.dgvJoints.RowHeadersVisible = false;
		this.dgvJoints.Size = new System.Drawing.Size(214, 123);
		this.dgvJoints.TabIndex = 10;
		base.Controls.Add(this.dgvJoints);
		base.Name = "SelectJointPrincipauxPopupForm";
		base.Size = new System.Drawing.Size(247, 147);
		((System.ComponentModel.ISupportInitialize)this.dgvJoints).EndInit();
		base.ResumeLayout(false);
	}

	private void SelectJoint(int rowIndex)
	{
		if (rowIndex < 0)
		{
			return;
		}
		DataGridViewRow dataGridViewRow = dgvJoints.Rows[rowIndex];
		if ((dataGridViewRow.Cells[0].Value?.ToString() ?? "") != string.Empty)
		{
			dataGridViewRow.Cells[0].Value = string.Empty;
			return;
		}
		SIGJoint sIGJoint = dataGridViewRow.Tag as SIGJoint;
		bool flag = false;
		bool flag2 = false;
		foreach (DataGridViewRow item in (IEnumerable)dgvJoints.Rows)
		{
			string text = (string)item.Cells[0].Value;
			if (text == "D")
			{
				flag = true;
			}
			else if (text == "F")
			{
				flag2 = true;
			}
		}
		if (flag && flag2)
		{
			MessageBox.Show("Les joints début et fin sont déjà définis");
		}
		List<SIGSegment> list = new List<SIGSegment>();
		foreach (SIGJoint joint in Joints)
		{
			if (joint != sIGJoint)
			{
				list = CDV_Viewer.Traitements.Composants.FindParcours(sIGJoint, joint);
				if (list.Count > 0)
				{
					break;
				}
			}
		}
		if (list.Count > 0)
		{
			if (list[0].PkD < list[0].PkF)
			{
				dataGridViewRow.Cells[0].Value = ((!flag) ? "D" : "F");
			}
			else
			{
				dataGridViewRow.Cells[0].Value = ((!flag2) ? "F" : "D");
			}
		}
		else
		{
			MessageBox.Show("Impossible de déterminer la position de ce joint");
		}
	}

	protected override void OnClosing(PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		JointDebut = null;
		JointFin = null;
		foreach (DataGridViewRow item in (IEnumerable)dgvJoints.Rows)
		{
			if ((string)item.Cells[0].Value == "D")
			{
				JointDebut = (SIGJoint)item.Tag;
			}
			else if ((string)item.Cells[0].Value == "F")
			{
				JointFin = (SIGJoint)item.Tag;
			}
		}
		if (JointDebut == null)
		{
			MessageBox.Show("Le joint de début n'est pas définit");
			e.Canceled = true;
		}
		else if (JointFin == null)
		{
			MessageBox.Show("Le joint de fin n'est pas définit");
			e.Canceled = true;
		}
	}
}
