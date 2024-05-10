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
    public partial class Form1 : Form
    {
        // Dichiarazione di variabili globali
        string[] paths, files;
        bool isRandom = false;
        bool isRepeat = false;

        public Form1()
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
            // Carica la traccia selezionata nel player
            player.URL = paths[index];
            player.Ctlcontrols.play();

            try
            {
                // Ottieni le informazioni sul file audio utilizzando TagLib#
                var file = TagLib.File.Create(paths[index]);

                // Imposta la massima posizione della trackbar in base alla durata della traccia
                tkb_andamento.Maximum = (int)file.Properties.Duration.TotalSeconds;

                // Imposta il testo delle label con il nome della canzone e dell'artista
                string songName = file.Tag.Title;
                string artistName = file.Tag.FirstPerformer;

                // Imposta il testo delle label con il nome della canzone e dell'artista
                if (!string.IsNullOrEmpty(songName))
                    lbl_songname.Text = songName;
                else
                    lbl_songname.Text = "N/A";

                if (!string.IsNullOrEmpty(artistName))
                    lbl_artistname.Text = artistName;
                else
                    lbl_artistname.Text = "N/A";

                // Carica l'immagine dell'album se disponibile
                if (file.Tag.Pictures.Length > 0)
                {
                    var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                    pct_track.Image = Image.FromStream(new MemoryStream(bin));
                }
                else
                {
                    // Se non ci sono immagini disponibili, puoi impostare un'immagine predefinita o lasciare vuota
                    pct_track.Image = null;
                }
            }
            catch (Exception ex)
            {
                // Gestisci eventuali errori
                MessageBox.Show("Error: " + ex.Message);
                lbl_songname.Text = "N/A";
                lbl_artistname.Text = "N/A";
                pct_track.Image = null;
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
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        private void pct_minimize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
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
                    lst_tracklist.Items.Add(files[x]);
                }
            }
        }

        private void pct_play_Click(object sender, EventArgs e)
        {
            player.Ctlcontrols.pause();
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

                // Avvia la riproduzione della nuova traccia
                player.Ctlcontrols.play();
            }
            else
            {
                // Se siamo alla fine della playlist, interrompi la riproduzione
                player.Ctlcontrols.stop();
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
                // Verifica se il player sta effettivamente riproducendo qualcosa
                if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    // Aggiorna l'etichetta di fine traccia
                    lbl_track_end.Text = player.Ctlcontrols.currentItem.durationString;

                    // Se il timer è attivo e il player sta effettivamente riproducendo qualcosa, aggiorna l'etichetta di inizio traccia
                    lbl_track_start.Text = player.Ctlcontrols.currentPositionString;
                }
                else
                {
                    // Se il player non sta riproducendo nulla, reimposta le etichette di inizio e fine traccia a "N/A"
                    lbl_track_start.Text = "N/A";
                    lbl_track_end.Text = "N/A";

                    // Se il repeat è attivo, riavvia la traccia corrente
                    if (isRepeat)
                    {
                        LoadAndPlayTrack(lst_tracklist.SelectedIndex);
                    }
                    else if (isRandom)
                    {
                        // Se la riproduzione casuale è attiva, riproduci una traccia casuale
                        Random rand = new Random();
                        int randomIndex = rand.Next(0, lst_tracklist.Items.Count);
                        lst_tracklist.SelectedIndex = randomIndex;
                        LoadAndPlayTrack(randomIndex);
                    }

                    // Reimposta la trackbar alla fine di ogni traccia
                    tkb_andamento.Value = 0;
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
            if (lst_tracklist.SelectedIndex >= 0 && lst_tracklist.SelectedIndex < paths.Length)
            {
                // Carica e riproduce la traccia selezionata
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
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded && lst_tracklist.SelectedIndex >= 0)
            {
                // Verifica che lo stato sia quello di fine traccia
                string trackPath = paths[lst_tracklist.SelectedIndex];
                if (!trackPlayCounts.ContainsKey(trackPath))
                {
                    trackPlayCounts[trackPath] = 1;
                }
                else
                {
                    trackPlayCounts[trackPath]++;
                }

                // Salva i dati delle tracce riprodotte nel file di log
                SaveTrackPlayData(trackPath, trackPlayCounts[trackPath]);
            }
        }


        private void SaveTrackPlayData(string trackPath, int playCount)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("track_play_log.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now}: Traccia '{trackPath}' riprodotta {playCount} volte.");
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
                    // Leggi il file di log delle tracce riprodotte
                    string[] lines = File.ReadAllLines("track_play_log.txt");

                    // Visualizza le informazioni sull'attività
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
            MessageBox.Show("Not available yet");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not available yet");
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

            // Aggiungi un gestore per l'evento di fine riproduzione della traccia corrente
            player.PlayStateChange += Player_PlayStateChange;

            // Avvia la riproduzione della traccia
            player.Ctlcontrols.play();

            // Imposta il flag per indicare che la radio è stata avviata
            isRadioStarted = true;
        }

    }
}

