using System;
using System.Linq;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;

namespace ModHelper.UI.Elements
{
    public class LogPanel : BasePanel
    {
        public LogPanel() : base(header: "Log")
        {
            AddPadding(5);
            AddHeader(title: "Log",
                onLeftClick: Log.OpenLogFolder,
                hover: "Click to open the folder at Steam/steamapps/common/tModLoader/tModLoader-Logs");

            AddAction(Log.OpenClientLog, "Open client.log", "Click to open client.log");
            AddAction(Log.OpenServerLog, "Open server.log", "Click to open server.log");
            AddAction(Log.ClearClientLog, "Clear Log", "Clear the client.log file");
            AddAction(Log.OpenEnabledJson, "Open enabled.json", "Click to open enabled.json", Log.OpenEnabledJsonFolder);
            AddPadding();

            AddHeader(title: "Log Level", hover: "Set the log level for each logger (0-5): Off, Error, Warn, Info, Debug, All");

            // add all sliders for all loggers
            foreach (var log in LogManager.GetCurrentLoggers().OrderBy(l => l.Logger.Name))
            {
                AddSlider(
                    title: log.Logger.Name,
                    min: 0,
                    max: 5,
                    defaultValue: 5,
                    onValueChanged: (value) => SetLogLevel(value, log.Logger as Logger),
                    increment: 1,
                    textSize: 0.8f,
                    hover: $"Set the log level for {log.Logger.Name}",
                    valueFormatter: (value) => ((LogLevel)value).ToString()
                );
            }
        }

        [Flags]
        public enum LogLevel
        {
            Off = 0,
            Error = 1,
            Warn = 2,
            Info = 3,
            Debug = 4,
            All = 5
        }

        private static void SetLogLevel(float value, Logger logger)
        {
            if (logger == null)
                return;

            // Convert value from 0-5 to LogLevel enum
            LogLevel level = (LogLevel)value;

            logger.Level = level switch
            {
                LogLevel.Error => Level.Error,
                LogLevel.Warn => Level.Warn,
                LogLevel.Info => Level.Info,
                LogLevel.Debug => Level.Debug,
                LogLevel.All => Level.All,
                _ => Level.Off
            };
        }

        public override void Update(GameTime gameTime)
        {
            Log.SlowInfo("LogPanel Active: " + Active);
            if (!Active)
                return;

            base.Update(gameTime);
        }
    }
}