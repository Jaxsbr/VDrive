using System;

namespace JJDev.VDrive.Core.Bundling
{
  public class ProgressEventArgs : EventArgs
  {
    private readonly int _progressIndex = 0;
    private readonly int _maxProgress = 0;


    private int _percentageCompleted = -1;
    public int PercentageCompleted
    {
      get
      {
        if (_percentageCompleted <= 0)
        {
          double percentage = _progressIndex / _maxProgress * 100;
          _percentageCompleted = (int)Math.Round(percentage);
        }
        return _percentageCompleted;
      }
    }


    public ProgressEventArgs(int progressIndex, int maxProgress)
    {
      if (progressIndex < 0 || maxProgress < 0 || progressIndex > maxProgress)
      {
        throw new InvalidOperationException("Invalid input");
      }      

      _progressIndex = progressIndex;
      _maxProgress = maxProgress;
    }
  }
}