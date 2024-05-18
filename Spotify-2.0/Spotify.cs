using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotify_Latest
{
    public partial class Spotify : Form
    {
        // Dichiarazione di variabili globali
        string[] paths, files;
        bool isRandom = false;
        bool isRepeat = false;

        public Spotify()
        {
            InitializeComponent();
            player.settings.autoStart = false; // Imposta autoStart su false per avviare manualmente la riproduzione
            player.PlayStateChange += Player_PlayStateChange;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Impostazioni iniziali
            tkb_volume.Value = 20;
            lbl_volume.Text = "20%";
            timer1.Enabled = true;

            string logFilePath = "track_play_log.txt";

            // Check if the file exists, then delete it
            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante l'eliminazione del file di log: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void Spotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            string logFilePath = "track_play_log.txt";

            // Check if the file exists, then delete it
            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante l'eliminazione del file di log: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btn_playsong_Click(object sender, EventArgs e)
        {
            PlaySelectedTrack();
        }

        private void PlaySelectedTrack()
        {
            if (lst_tracklist.Items.Count == 0)
            {
                MessageBox.Show("Nessuna musica caricata!", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Seleziona la prima canzone nella lst_tracklist
                lst_tracklist.SelectedIndex = 0;

                // Carica e riproduce la traccia selezionata
                LoadAndPlayTrack(lst_tracklist.SelectedIndex);
            }
        }

        private void LoadAndPlayTrack(int index)
        {
            if (index >= 0 && index < paths.Length)
            {
                // Load the selected track into the player
                player.URL = paths[index];

                try
                {
                    var file = TagLib.File.Create(paths[index]);
                    tkb_andamento.Maximum = (int)file.Properties.Duration.TotalSeconds;

                    string songName = file.Tag.Title;
                    string artistName = file.Tag.FirstPerformer;

                    lbl_songname.Text = !string.IsNullOrEmpty(songName) ? songName : "N/A";
                    lbl_artistname.Text = !string.IsNullOrEmpty(artistName) ? artistName : "N/A";

                    if (file.Tag.Pictures.Length > 0)
                    {
                        var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                        pct_track.Image = Image.FromStream(new MemoryStream(bin));
                    }
                    else
                    {
                        pct_track.Image = null;
                    }

                    // Start playing the track
                    player.Ctlcontrols.play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    lbl_songname.Text = "N/A";
                    lbl_artistname.Text = "N/A";
                    pct_track.Image = null;
                }
            }
        }





        private void btn_playalbum_Click(object sender, EventArgs e)
        {
            PlaySelectedTrack();
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            rtxt_search.Text = "";
        }

        private void rtxt_search_TextChanged(object sender, EventArgs e)
        {
            // Implementazione della ricerca
        }

        private void pct_close_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tkb_volume_Scroll(object sender, EventArgs e)
        {
            player.settings.volume = tkb_volume.Value;
            lbl_volume.Text = tkb_volume.Value.ToString() + "%";
        }

        private void pct_maximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Minimized;
        }

        private void pct_minimize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Minimized;

            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
        }

        private void btn_localfiles_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                files = ofd.FileNames;
                paths = ofd.FileNames;

                for (int x = 0; x < files.Length; x++)
                {
                    try
                    {
                        var file = TagLib.File.Create(files[x]);
                        string songName = file.Tag.Title ?? Path.GetFileName(files[x]); // Use file name if song title is not available
                        lst_tracklist.Items.Add(songName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Errore durante il caricamento del file: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void pct_play_Click(object sender, EventArgs e)
        {
            if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                player.Ctlcontrols.pause();
            }
            else if (player.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                player.Ctlcontrols.play();
            }
            else
            {
                if (lst_tracklist.SelectedIndex >= 0)
                {
                    LoadAndPlayTrack(lst_tracklist.SelectedIndex);
                    player.Ctlcontrols.play();
                }
                else
                {
                    MessageBox.Show("Nessuna traccia selezionata.", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        private void pct_preview_Click(object sender, EventArgs e)
        {
            PlayPreviousTrack();
        }

        private void PlayPreviousTrack()
        {
            if (lst_tracklist.SelectedIndex > 0)
            {
                lst_tracklist.SelectedIndex = lst_tracklist.SelectedIndex - 1;
                LoadAndPlayTrack(lst_tracklist.SelectedIndex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // Update the track status labels and trackbar control even when paused
                if (player.playState == WMPLib.WMPPlayState.wmppsPlaying || player.playState == WMPLib.WMPPlayState.wmppsPaused)
                {
                    // Update the end track label
                    lbl_track_end.Text = player.Ctlcontrols.currentItem.durationString;

                    // Update the start track label if the timer is active and the player is actually playing something
                    lbl_track_start.Text = player.Ctlcontrols.currentPositionString;
                }

                if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    // Update the trackbar control position only when playing
                    tkb_andamento.Value = (int)player.Ctlcontrols.currentPosition;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void tkb_andamento_MouseUp(object sender, MouseEventArgs e)
        {
            // Riattiva il timer dopo aver rilasciato il cursore della trackbar
            timer1.Enabled = true;
        }

        private void tkb_andamento_MouseDown(object sender, MouseEventArgs e)
        {
            // Disattiva il timer durante lo spostamento del cursore della trackbar
            timer1.Enabled = false;

            // Imposta la posizione corrente del player
            player.Ctlcontrols.currentPosition = tkb_andamento.Value;
        }

        private void tkb_andamento_Scroll(object sender, EventArgs e)
        {
            // Imposta la posizione corrente del player in base alla posizione della trackbar
            player.Ctlcontrols.currentPosition = tkb_andamento.Value;

            // Aggiorna manualmente l'etichetta di inizio traccia
            lbl_track_start.Text = TimeSpan.FromSeconds(tkb_andamento.Value).ToString(@"mm\:ss");
        }

        private void lst_tracklist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_tracklist.SelectedIndex >= 0)
            {
                LoadAndPlayTrack(lst_tracklist.SelectedIndex);
            }
        }


        // Implementazioni aggiuntive

        private void btn_savePlaylist_Click(object sender, EventArgs e)
        {
            // Implementazione per salvare la playlist
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            // Implementazione per la ricerca
        }

        private void pct_loop_Click(object sender, EventArgs e)
        {
            isRepeat = !isRepeat;
        }

        private void pct_shuffle_Click(object sender, EventArgs e)
        {
            isRandom = !isRandom;
        }

        private void btn_saveplaylist_Click_1(object sender, EventArgs e)
        {
            // Verifica se ci sono tracce nella lista
            if (lst_tracklist.Items.Count == 0)
            {
                MessageBox.Show("Nessuna traccia nella playlist da salvare!", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Apre il dialogo di salvataggio per chiedere all'utente dove salvare la playlist
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Playlist files (*.m3u)|*.m3u|All files (*.*)|*.*";
            saveFileDialog.Title = "Salva playlist";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Ottiene il percorso del file selezionato
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Scrive le informazioni delle tracce nella playlist
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (string trackPath in paths)
                        {
                            writer.WriteLine(trackPath);
                        }
                    }

                    MessageBox.Show("Playlist salvata correttamente!", "Spotify: Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante il salvataggio della playlist: " + ex.Message, "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_loadplaylist_Click(object sender, EventArgs e)
        {
            // Apre il dialogo di apertura file per selezionare la playlist da caricare
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Playlist files (*.m3u)|*.m3u|All files (*.*)|*.*";
            openFileDialog.Title = "Carica playlist";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Ottiene il percorso del file selezionato
                string filePath = openFileDialog.FileName;

                try
                {
                    // Legge le tracce dalla playlist e le aggiunge alla lista
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lst_tracklist.Items.Add(line);
                        }
                    }

                    // Aggiorna anche l'array di percorsi delle tracce
                    paths = lst_tracklist.Items.Cast<string>().ToArray();

                    MessageBox.Show("Playlist caricata correttamente!", "Spotify: Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante il caricamento della playlist: " + ex.Message, "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool isRadioStarted = false;

        private void Player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                string trackPath = paths[lst_tracklist.SelectedIndex];
                if (!trackPlayCounts.ContainsKey(trackPath))
                {
                    trackPlayCounts[trackPath] = 1;
                }
                else
                {
                    trackPlayCounts[trackPath]++;
                }

                // Save the play data to the log file
                SaveTrackPlayData(trackPath, trackPlayCounts[trackPath]);
            }
            else if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                PlayNextTrack();
            }
        }





        private void SaveTrackPlayData(string trackPath, int playCount)
        {
            try
            {
                // Get the song name using TagLib#
                string songName;
                try
                {
                    var file = TagLib.File.Create(trackPath);
                    songName = file.Tag.Title ?? "Unknown";
                }
                catch
                {
                    songName = "Unknown";
                }

                using (StreamWriter sw = new StreamWriter("track_play_log.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now}: Traccia '{songName}' riprodotta {playCount} volte.\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il salvataggio dei dati di riproduzione della traccia: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Dictionary<string, int> trackPlayCounts = new Dictionary<string, int>();

        private void btn_activity_Click(object sender, EventArgs e)
        {
            // Verifica se ci sono tracce nella lista
            if (lst_tracklist.Items.Count == 0)
            {
                MessageBox.Show("Nessuna traccia nella playlist!", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mostra le informazioni sull'attività
            ShowActivityInfo();
        }

        private void ShowActivityInfo()
        {
            try
            {
                if (File.Exists("track_play_log.txt"))
                {
                    // Read the log file
                    string[] lines = File.ReadAllLines("track_play_log.txt");

                    // Display the activity information
                    StringBuilder message = new StringBuilder();
                    foreach (string line in lines)
                    {
                        message.AppendLine(line);
                    }
                    MessageBox.Show(message.ToString(), "Attività", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Nessuna attività da mostrare.", "Attività", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante la lettura dei dati di attività: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btn_artists_Click(object sender, EventArgs e)
        {
            // Check if there are tracks loaded
            if (lst_tracklist.Items.Count == 0)
            {
                MessageBox.Show("Nessuna traccia nella playlist!", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a HashSet to store unique artist names
            HashSet<string> artists = new HashSet<string>();

            // Iterate through the loaded tracks to extract artist names
            foreach (string trackPath in paths)
            {
                try
                {
                    var file = TagLib.File.Create(trackPath);
                    string artistName = file.Tag.FirstPerformer;

                    // Add the artist name to the HashSet
                    if (!string.IsNullOrEmpty(artistName))
                    {
                        artists.Add(artistName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante il caricamento del file: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Check if any artists were found
            if (artists.Count == 0)
            {
                MessageBox.Show("Nessun artista trovato!", "Spotify: Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Construct a message to display the list of artists
                StringBuilder message = new StringBuilder();
                message.AppendLine("Artisti nella playlist:");
                foreach (string artist in artists)
                {
                    message.AppendLine(artist);
                }

                // Show the message box with the list of artists
                MessageBox.Show(message.ToString(), "Artisti", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void btn_biography_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not available yet");
        }

        private void pct_next_Click(object sender, EventArgs e)
        {
            PlayNextTrack();
        }

        private void PlayNextTrack()
        {
            if (lst_tracklist.SelectedIndex < lst_tracklist.Items.Count - 1)
            {
                lst_tracklist.SelectedIndex = lst_tracklist.SelectedIndex + 1;
                LoadAndPlayTrack(lst_tracklist.SelectedIndex);
                player.Ctlcontrols.play(); // Start playing the next track
            }
            else
            {
                player.Ctlcontrols.stop();
            }
        }







        private void Spotify_Leave(object sender, EventArgs e)
        {

        }

        private void btn_biography_Click_1(object sender, EventArgs e)
        {
            // Check if there's a selected artist
            if (lst_tracklist.SelectedItem == null)
            {
                MessageBox.Show("Nessun artista selezionato!", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the selected artist name
            string selectedArtist = lst_tracklist.SelectedItem.ToString();

            // Fetch biography for the selected artist (you would need to implement a method to fetch biographies)
            string biography = GetArtistBiography(selectedArtist);

            // Check if biography is available
            if (string.IsNullOrEmpty(biography))
            {
                MessageBox.Show($"Biografia non disponibile per {selectedArtist}", "Spotify: Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Show the biography in a message box
                MessageBox.Show(biography, $"{selectedArtist}: Biografia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Method to fetch biography for an artist (you need to implement this)
        private string GetArtistBiography(string artistName)
        {
            // Implement logic to fetch biography from a source like an API or a database - TO DO NEXT
            return "Biografia di " + artistName;
        }


        private void btn_radio_Click_1(object sender, EventArgs e)
        {
            // Verifica se la riproduzione è già in corso
            if (isRadioStarted)
            {
                MessageBox.Show("La riproduzione è già in corso!", "Spotify: Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Verifica se ci sono tracce nella lista
            if (lst_tracklist.Items.Count == 0)
            {
                MessageBox.Show("Nessuna traccia nella playlist!", "Spotify: Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Avvia la riproduzione della prima traccia
            lst_tracklist.SelectedIndex = 0;
            PlaySelectedTrack();

            // Imposta il flag per indicare che la radio è stata avviata
            isRadioStarted = true;
        }


    }
}

