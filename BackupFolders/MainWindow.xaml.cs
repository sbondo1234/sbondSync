﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPFCustomMessageBox;

namespace BackupFolders
{
    public partial class MainWindow : Window
    {
        public static string BackupDir;

        // Colors
        byte ErrorA = 255;
        byte ErrorR = 125;
        byte ErrorG = 39;
        byte ErrorB = 39;

        byte SuccessA = 255;
        byte SuccessR = 95;
        byte SuccessG = 186;
        byte SuccessB = 125;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BackupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void FileCheckBox_Click(object sender, RoutedEventArgs e)
        {
            FolderCheckBox.IsChecked = false;
        }

        private void FolderCheckBox_Click(object sender, RoutedEventArgs e)
        {
            FileCheckBox.IsChecked = false;
        }

        private void NoticeTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            NoticeTextBlock.Text = "";
            NoticeTextBlock.Opacity = 0;
            NoticeTextBlock.Cursor = Cursors.Arrow;
        }

        private void Notice(string Error, byte A, byte R, byte G, byte B)
        {
            NoticeTextBlock.Text = $"{Error}";
            NoticeTextBlock.Foreground = new SolidColorBrush(Color.FromArgb(A, R, G, B));
            NoticeTextBlock.Cursor = Cursors.Hand;
            NoticeTextBlock.Opacity = 100;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectingFilesClass.SelectFiles(FileCheckBox, FolderCheckBox, FolderDirTextBox);
        }

        private void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileCheckBox.IsChecked == true)
            {
                if (!SelectedFilesListBox.Items.Contains(FolderDirTextBox.Text))
                {
                    SelectedFilesListBox.Items.Add(FolderDirTextBox.Text);
                }
                else
                {
                    Notice("Error: File(s) already added.", ErrorA, ErrorR, ErrorG, ErrorB);
                }
            }
            else if (FolderCheckBox.IsChecked == true)
            {
                if (!SelectedFilesListBox.Items.Contains(FolderDirTextBox.Text))
                {
                    SelectedFilesListBox.Items.Add(FolderDirTextBox.Text);
                }
                else
                {
                    Notice("Error: File(s) already added.", ErrorA, ErrorR, ErrorG, ErrorB);
                }
            }
            ShouldBackupFilesButtonBePlural();
        }

        private void ShouldBackupFilesButtonBePlural()
        {
            if (SelectedFilesListBox.Items.Count > 1)
            {
                BackupFilesButton.Content = "Backup Files";
            }
            else
            {
                BackupFilesButton.Content = "Backup File";
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilesListBox.Items.Clear();
            ShouldBackupFilesButtonBePlural();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string s in SelectedFilesListBox.SelectedItems.OfType<string>().ToList())
            {
                SelectedFilesListBox.Items.Remove(s);
            }
            ShouldBackupFilesButtonBePlural();
        }

        public static string IsDefaultBackupDirAssigned()
        {
            if (Properties.Settings.Default.DefaultSaveDir != "")
            {
                BackupDir = Properties.Settings.Default.DefaultSaveDir;
                MessageBoxResult result = CustomMessageBox.Show($"Backup to: '{BackupDir}' ?", "Backup Directory",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // CustomMessageBox.Show("Yes");
                        break;
                    case MessageBoxResult.No:
                        SelectBackupDir();
                        break;
                }
            }
            else
            {
                CustomMessageBox.Show("Please select a default backup directory." +
                "\nThis can be changed later in the settings.", "Backup Dir Not Set");
                SelectBackupDir();
            }
            return BackupDir;
        }

        public static string SelectBackupDir()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Properties.Settings.Default.DefaultSaveDir = fbd.SelectedPath;
                    BackupDir = Properties.Settings.Default.DefaultSaveDir;
                }
            }
            Properties.Settings.Default.Save();
            return BackupDir;
        }

        private void BackupFilesButton_Click(object sender, RoutedEventArgs e)
        {
            FileCopyingClass.StartCopying(SelectedFilesListBox, ProgressBar);
        }
    }
}
