using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpriteClasses
{
    public class Animation
    {
        public int CurrentCell { get; set; }

        private bool Looping { get; set; }
        private bool Stopped { get; set; }
        private bool Playing { get; set; }
        // Time we need to goto next frame
        private float TimeShift { get; set; }
        // Time since last shift
        private float TotalTime { get; set; }
        private int Start { get; set; }
        private int End { get; set; }
        public List<Texture2D> cellList;
        
        public Animation()
        {
            cellList = new List<Texture2D>();
        }
        //add animation image to list
        public void AddCell(Texture2D cellPicture)
        {
            cellList.Add(cellPicture);
        }

        //************NOTE*********************
        //**Setup methods get called once from UpdateInput to set up the animation
        //**Update gets called from Game Update method, it keeps things going

        //set up to play entire sequence and keep looping through it,
        //seconds parameter is how long the entire sequence should take
        public void LoopAll(float seconds)
        {
            if (Playing) return;
            
            Stopped = false;
            if (Looping) return;

            Looping = true;
            Start = 0;
            End = cellList.Count - 1;

            CurrentCell = Start;
            //time for each cell to show
            TimeShift = seconds / (float)cellList.Count; 
        }
        //set up for loop through some of the images only,
        //seconds parameter is how long the whole sequence should take
        public void Loop(int start, int end, float seconds )
        {
            if (Playing) return;

            Stopped = false;
            if (Looping) return;

            Looping = true;
            this.Start = start;
            this.End = end;

            CurrentCell = start;
            //set timing
            float difference = (float)end - (float)start;
            TimeShift = seconds / difference;
        }
        //set up to stop playing animations
        public void Stop()
        {
            if (Playing) return;

            Stopped = true;
            Looping = false;

            //set timing
            TotalTime = 0.0f;
            TimeShift = 0.0f;
        }
        //display just one frame
        public void GotoFrame(int number)
        {
            if (Playing) return;

            if (number < 0 || number >= cellList.Count) return;
            CurrentCell = number;
        }
        //set up to play entire sequence once only,
        //seconds parameter is how long the entire sequence should take
        public void PlayAll(float seconds)
        {
            if (Playing) return;

            GotoFrame(0);
            Stopped = false;
            Looping = false;
            Playing = true;

            Start = 0;
            End = cellList.Count - 1;

            //set timing
            TimeShift = seconds / (float)cellList.Count; 
        }
        //set up to play part of the sequence once only,
        //seconds parameter is how long the entire sequence should take
        public void Play(int start, int end, float seconds)
        {
            if (Playing) return;

            GotoFrame(start);
            Stopped = false;
            Looping = false;
            Playing = true;

            this.Start = start;
            this.End = end;

            //set timing
            float difference = (float)end - (float)start;
            TimeShift = seconds / difference;
        }
        //called once setup is done, to keep updating
        //which frame is shown, and to restart if looping
        //and the last frame has been reached
        public void Update(GameTime gameTime)
        {
            if (Stopped) return;
            //don't change frame unless enough time has elapsed
            TotalTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (TotalTime > TimeShift)
            {
                TotalTime = 0.0f;
                //increment cell to draw
                CurrentCell++;
                //if looping and we're on the last frame, 
                //reset to first one again
                if (Looping)
                {
                    if (CurrentCell > End) CurrentCell = Start;
                }
                //if we're at the end of the animation cycle, 
                //set booleans to indicate it
                if (CurrentCell > End)
                {
                    CurrentCell = End;
                    Playing = false;
                    Stopped = true;
                }
            }
        }
    }
}
