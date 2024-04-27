using Android.Content.PM;
using Android.Views;
using static Android.Graphics.PathIterator;
using System.Timers;
using System.Xml;
using Android.Graphics;
using Newtonsoft.Json;

namespace Football_TimeTracker_Android
{
    [Activity( Label = "@string/app_name", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape )]
    public class MainActivity : Activity
    {
        TextView? mainText, gamestatusText;
        EditText? gamenameText, competitionName;
        Button? activeButton, outofboundsButton, refblowButton, goalButton, undoButton, startButton, saveButton, resetButton;

        System.Timers.Timer? timerPrincipal;
        int seconds, half, currentSegmentType;
        List<Segment>? segments;
        bool ticking;

        protected override void OnCreate( Bundle? savedInstanceState )
        {
            base.OnCreate( savedInstanceState );

            // Set our view from the "main" layout resource
            SetContentView( Resource.Layout.activity_main );
            LoadElements();
            ResetAll();
        }

        private void LoadElements()
        {
            mainText = FindViewById<TextView>( Resource.Id.timeText );
            gamestatusText = FindViewById<TextView>( Resource.Id.gamestatusText );
            gamenameText = FindViewById<EditText>( Resource.Id.namegameText );
            competitionName = FindViewById<EditText>( Resource.Id.competitionText );
            activeButton = FindViewById<Button>( Resource.Id.ActiveButton );
            activeButton!.Click += OnActiveButtonClicked;
            outofboundsButton = FindViewById<Button>( Resource.Id.OutOfBoundsButton );
            outofboundsButton!.Click += OnOutOfBoundsButtonClicked;
            refblowButton = FindViewById<Button>( Resource.Id.RefBlowButton );
            refblowButton!.Click += OnRefBlowButtonClicked;
            goalButton = FindViewById<Button>( Resource.Id.GoalButton );
            goalButton!.Click += OnGoalButtonClicked;
            undoButton = FindViewById<Button>( Resource.Id.UndoButton );
            undoButton!.Click += OnUndoButtonClicked;
            startButton = FindViewById<Button>( Resource.Id.StartGameButton );
            startButton!.Click += OnStartGameButtonClicked;
            saveButton = FindViewById<Button>( Resource.Id.SaveButton );
            saveButton!.Click += OnSaveGameButtonClicked;
            resetButton = FindViewById<Button>( Resource.Id.ResetButton );
            resetButton!.Click += OnResetButtonClicked;

            Window!.SetFlags( WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen );
            ActionBar!.Hide();

            timerPrincipal = new System.Timers.Timer();
            timerPrincipal.Interval = 1000;
            timerPrincipal.Elapsed += new ElapsedEventHandler( TimerSecondPassed );
        }

        private void ResetAll()
        {
            half = 0;
            seconds = 0;
            segments = new List<Segment>();
            currentSegmentType = 0;
            ticking = false;
            timerPrincipal!.Stop();
            mainText!.Text = GetString(Resource.String.string_timer0);
            mainText.SetBackgroundColor( Constants.colorSegmentActive );
            gamestatusText!.Text = GetString( Resource.String.string_status_poriniciar ); ;
            gamestatusText.SetTextColor( Color.Black );
            gamestatusText.SetBackgroundColor( Constants.colorBackgroundGray );
            undoButton!.Enabled = false;
            undoButton.Background!.SetTint( Constants.disabledButton );
            undoButton.SetTextColor( Constants.disabledText );
            startButton!.Enabled = true;
            startButton.Background!.SetTint( Constants.enabledButton );
            startButton.SetTextColor( Color.Black );
            startButton.Text = GetString( Resource.String.string_startfirsthalf ); ;
            gamenameText!.Text = string.Empty;
            gamenameText.Enabled = true;
            competitionName!.Text = string.Empty;
            competitionName.Enabled = true;
            gamenameText.RequestFocus();
            saveButton!.Enabled = false;
            saveButton.Background!.SetTint( Constants.disabledButton );
            saveButton.SetTextColor( Constants.disabledText );
            saveButton.Text = GetString( Resource.String.string_save ); ;
            Window!.ClearFlags( WindowManagerFlags.KeepScreenOn );
        }

        private void OnActiveButtonClicked( object? sender, EventArgs args )
        {
            if (!ticking)
                return;

            if (currentSegmentType == Constants.segmentTypeActive)
            {
                //do nothing
            }
            else
            {
                currentSegmentType = Constants.segmentTypeActive;
                mainText!.SetBackgroundColor( Constants.colorSegmentActive );
                AddSegment();
            }
        }

        private void OnOutOfBoundsButtonClicked( object? sender, EventArgs args )
        {
            if (!ticking)
                return;

            if (currentSegmentType == Constants.segmentTypeOutofBounds)
            {
                //do nothing
            }
            else
            {
                if (currentSegmentType != Constants.segmentTypeActive)
                {
                    ReplaceLastSegment( Constants.segmentTypeOutofBounds );
                }
                else
                {
                    currentSegmentType = Constants.segmentTypeOutofBounds;
                    mainText!.SetBackgroundColor( Constants.colorSegmentOutofBounds );
                    AddSegment();
                }
            }
        }

        private void OnRefBlowButtonClicked( object? sender, EventArgs args )
        {
            if (!ticking)
                return;

            if (currentSegmentType == Constants.segmentTypeRefBlow)
            {
                //do nothing
            }
            else
            {
                if (currentSegmentType != Constants.segmentTypeActive)
                {
                    ReplaceLastSegment( Constants.segmentTypeRefBlow );
                }
                else
                {
                    currentSegmentType = Constants.segmentTypeRefBlow;
                    mainText!.SetBackgroundColor( Constants.colorSegmentRefBlow );
                    AddSegment();
                }
            }
        }

        private void OnGoalButtonClicked( object? sender, EventArgs args )
        {
            if (!ticking)
                return;

            if (currentSegmentType == Constants.segmentTypeGoal)
            {
                //do nothing
            }
            else
            {
                if (currentSegmentType != Constants.segmentTypeActive)
                {
                    ReplaceLastSegment( Constants.segmentTypeGoal );
                }
                else
                {
                    currentSegmentType = Constants.segmentTypeGoal;
                    mainText!.SetBackgroundColor( Constants.colorSegmentGoal );
                    AddSegment();
                }
            }
        }

        private void OnUndoButtonClicked( object? sender, EventArgs args )
        {
            if (!ticking)
            { return; }

            if (segments!.Where( x => x.half == half ).Count() > 1)
            {
                RemoveSegment();
            }
        }

        private void OnStartGameButtonClicked( object? sender, EventArgs args )
        {
            if (!ticking && half == 0)
            {
                seconds = 0;
                timerPrincipal!.Start();
                mainText!.SetBackgroundColor( Constants.colorSegmentActive );
                mainText.Text = GetString( Resource.String.string_timer0 );
                ticking = true;
                Window!.AddFlags( WindowManagerFlags.KeepScreenOn );
                currentSegmentType = Constants.segmentTypeActive;
                gamestatusText!.SetBackgroundColor( Color.Lime );
                gamestatusText.Text = GetString( Resource.String.string_status_firsthalfrunning );
                startButton!.Text = GetString( Resource.String.string_endfirsthalf );
                AddSegment();
            }
            else if (ticking && half == 0)
            {
                timerPrincipal!.Stop();
                half++;
                gamestatusText!.SetBackgroundColor( Color.Khaki );
                gamestatusText.Text = GetString( Resource.String.string_status_halftime );
                ticking = false;
                Window!.ClearFlags( WindowManagerFlags.KeepScreenOn );
                startButton!.Text = GetString( Resource.String.string_startsecondhalf );
                CheckUndoButton();
            }
            else if (!ticking && half == 1)
            {
                seconds = 0;
                timerPrincipal!.Start();
                mainText!.SetBackgroundColor( Constants.colorSegmentActive );
                mainText.Text = GetString( Resource.String.string_timer0 );
                ticking = true;
                Window!.AddFlags( WindowManagerFlags.KeepScreenOn );
                currentSegmentType = Constants.segmentTypeActive;
                gamestatusText!.SetBackgroundColor( Color.Lime );
                gamestatusText.Text = GetString( Resource.String.string_status_secondhalfrunning );
                startButton!.Text = GetString( Resource.String.string_endsecondhalf );
                AddSegment();
            }
            else if (ticking && half == 1)
            {
                timerPrincipal!.Stop();
                half++;
                gamestatusText!.SetBackgroundColor( Color.Red );
                gamestatusText.Text = GetString( Resource.String.string_status_fulltime );
                ticking = false;
                Window!.ClearFlags( WindowManagerFlags.KeepScreenOn );
                startButton!.Background!.SetTint( Constants.disabledButton );
                startButton.SetTextColor( Constants.disabledText );
                startButton.Enabled = false;
                saveButton!.Enabled = true;
                saveButton.Background!.SetTint( Constants.enabledButton );
                saveButton.SetTextColor( Color.Black );
                CheckUndoButton();
            }
        }

        private void OnSaveGameButtonClicked( object? sender, EventArgs args )
        {
            if (gamenameText!.Text!.Trim().Length == 0 || competitionName!.Text!.Trim().Length == 0)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder( this );
                builder.SetTitle( GetString( Resource.String.string_namepopup_title ) );
                builder.SetMessage( GetString( Resource.String.string_namepopup_message ) );
                builder.SetPositiveButton( GetString( Resource.String.OK ), ( senderAlert, args ) => { } );

                AlertDialog dialog = builder.Create()!;
                dialog!.Show();
            }
            else
            {
                DateTime dateTime = DateTime.Now;
                string fileName = gamenameText.Text.Trim() + "_" + competitionName.Text.Trim() + "_" + dateTime.Date.ToString( "dd-MM-yyyy" );
                string filesName = System.IO.Path.Combine( Android.OS.Environment.GetExternalStoragePublicDirectory( Android.OS.Environment.DirectoryDownloads )!.Path, fileName + ".txt" );
                string json = JsonConvert.SerializeObject( segments, Newtonsoft.Json.Formatting.None );
                File.WriteAllText( filesName, json );
                AlertDialog.Builder builder = new AlertDialog.Builder( this );
                builder.SetTitle( GetString( Resource.String.string_savepopup_title ) );
                builder.SetMessage( GetString( Resource.String.string_savepopup_message ) + filesName );
                builder.SetPositiveButton( GetString( Resource.String.OK ), ( senderAlert, args ) => { } );
                AlertDialog dialog = builder.Create()!;
                dialog!.Show();
                saveButton!.Enabled = false;
                saveButton.Background!.SetTint( Constants.disabledButton );
                saveButton.SetTextColor( Constants.disabledText );
                saveButton.Text = GetString( Resource.String.string_gamesaved );
                gamenameText.Enabled = false;
                competitionName.Enabled = false;
            }
        }

        private void OnResetButtonClicked( object? sender, EventArgs args )
        {
            AlertDialog.Builder builder = new AlertDialog.Builder( this );
            builder.SetMessage( GetString( Resource.String.string_resetpopup_title ) );
            builder.SetPositiveButton( GetString( Resource.String.string_resetpopup_positive ), ( senderAlert, args ) => { ResetAll(); } );
            builder.SetNeutralButton( GetString( Resource.String.string_resetpopup_negative ), ( senderAlert, args ) => { } );

            AlertDialog dialog = builder.Create()!;
            dialog!.Show();
        }

        private void TimerSecondPassed( Object? myObject,
                                            EventArgs myEventArgs )
        {
            if (ticking)
            {
                seconds++;
                int formattedMinutes, formattedSeconds;
                formattedSeconds = seconds;
                formattedMinutes = 0;
                while (formattedSeconds >= 60)
                {
                    formattedSeconds -= 60;
                    formattedMinutes++;
                }
                if (seconds >= Constants.minute45) //45 mins
                {
                    formattedMinutes -= 45;
                    mainText!.Text = "45:00\n+ " + formattedMinutes.ToString( "D2" ) + ":" + formattedSeconds.ToString( "D2" );
                }
                else
                {
                    mainText!.Text = formattedMinutes.ToString( "D2" ) + ":" + formattedSeconds.ToString( "D2" );
                }

                segments!.LastOrDefault()!.addSeconds();

                UpdateTotals();
            }
        }

        private void AddSegment()
        {
            if (GarbageCollected())
            {
                if (currentSegmentType == segments!.Last().segmentType)
                {
                    return;
                }
            }

            Segment segment = new Segment( seconds, currentSegmentType, half );
            segments!.Add( segment );

            CheckUndoButton();
        }

        private bool GarbageCollected()
        {
            if (segments!.Where( x => x.half == half).Count() > 1)
            {
                Segment lastSegment = segments.Last();
                if (lastSegment.elapsedSeconds == 0)
                {
                    segments.Remove( lastSegment );

                    return true;
                }
            }
            return false;
        }

        private void ReplaceLastSegment( int segmentType )
        {
            segments!.Last().segmentType = segmentType;
            currentSegmentType = segmentType;
            switch (currentSegmentType)
            {
                case Constants.segmentTypeActive:
                    mainText!.SetBackgroundColor( Constants.colorSegmentActive );
                    break;
                case Constants.segmentTypeOutofBounds:
                    mainText!.SetBackgroundColor( Constants.colorSegmentOutofBounds );
                    break;
                case Constants.segmentTypeRefBlow:
                    mainText!.SetBackgroundColor( Constants.colorSegmentRefBlow );
                    break;
                case Constants.segmentTypeGoal:
                    mainText!.SetBackgroundColor( Constants.colorSegmentGoal );
                    break;
            }
        }

        private void RemoveSegment()
        {
            Segment lastsegment = segments!.Last();
            segments!.Remove( lastsegment );
            Segment prelastsegment = segments.Last();
            prelastsegment.elapsedSeconds += lastsegment.elapsedSeconds;
            currentSegmentType = prelastsegment.segmentType;
            switch (currentSegmentType)
            {
                case Constants.segmentTypeActive:
                    mainText!.SetBackgroundColor( Constants.colorSegmentActive );
                    break;
                case Constants.segmentTypeOutofBounds:
                    mainText!.SetBackgroundColor( Constants.colorSegmentOutofBounds );
                    break;
                case Constants.segmentTypeRefBlow:
                    mainText!.SetBackgroundColor( Constants.colorSegmentRefBlow );
                    break;
                case Constants.segmentTypeGoal:
                    mainText!.SetBackgroundColor( Constants.colorSegmentGoal );
                    break;
            }
            CheckUndoButton();
        }

        private void CheckUndoButton()
        {
            if(!ticking)
            {
                undoButton!.Enabled = false;
                undoButton.Background!.SetTint( Constants.disabledButton );
                undoButton.SetTextColor( Constants.disabledText );
            }

            if (segments!.Where( x => x.half == half ).ToList().Count > 1)
            {
                undoButton!.Enabled = true;
                undoButton.Background!.SetTint( Constants.enabledButton );
                undoButton.SetTextColor( Color.Black );
            }
            else
            {
                undoButton!.Enabled = false;
                undoButton.Background!.SetTint( Constants.disabledButton );
                undoButton.SetTextColor( Constants.disabledText );
            }
        }

        private void UpdateTotals()
        {
            switch (currentSegmentType)
            {
                case Constants.segmentTypeActive:
                    mainText!.SetBackgroundColor( Constants.colorSegmentActive );
                    break;
                case Constants.segmentTypeOutofBounds:
                    mainText!.SetBackgroundColor( Constants.colorSegmentOutofBounds );
                    break;
                case Constants.segmentTypeRefBlow:
                    mainText!.SetBackgroundColor( Constants.colorSegmentRefBlow );
                    break;
                case Constants.segmentTypeGoal:
                    mainText!.SetBackgroundColor( Constants.colorSegmentGoal );
                    break;
            }
        }
    }

    static class Constants
    {
        public const int segmentTypeActive = 0;
        public const int segmentTypeOutofBounds = 1;
        public const int segmentTypeRefBlow = 2;
        public const int segmentTypeGoal = 3;

        public const int minute45 = 2700;

        public static Color colorSegmentActive = Color.SpringGreen;
        public static Color colorSegmentOutofBounds = Color.Yellow;
        public static Color colorSegmentRefBlow = Color.Salmon;
        public static Color colorSegmentGoal = Color.LightSkyBlue;

        public static Color colorBackgroundGray = Color.DimGray;
        public static Color enabledButton = Color.Rgb( 240, 240, 240 );
        public static Color disabledButton = Color.Rgb( 144, 144, 144 );
        public static Color disabledText = Color.Rgb( 100, 100, 100 );
    }

    public class Segment
    {
        public int elapsedSeconds;
        public int startingSeconds;
        public int segmentType;
        public int half;

        public Segment( int startingSeconds, int segmenttype, int half )
        {
            this.startingSeconds = startingSeconds;
            this.segmentType = segmenttype;
            this.half = half;
            this.elapsedSeconds = 0;
        }

        public void addSeconds()
        {
            elapsedSeconds++;
        }
    }
}