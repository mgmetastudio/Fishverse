# Presentation  
  Component that handle time based progression, you may want to use this for plants
  growing for exemple. You can either use real time clock, or you own
  game clock.
  
<br>
<br>
  
# Required Components  
 - XpTracker
  
<br>
<br>
  
# Set Up  
 - 1 : Add the XpDistributor_TimeBased component to your prefab. It will automatically add Xp component too.  
 - 2 : Define Id (this should match and existing Experience track) and PointsPerSecond to be granted.
 - 3 : Call XpDistributor_TimeBased.Refresh() function in your own logic.
  
<br>
<br>
  
# Properties  
  
  ## Id : string
  Associated experience ID.

  ## PointsPerSecond : float
  Quantity of experience points given for every elasped second

<br>
<br>
  
# Methods
  ## Reset(long seconds)
  Reset internal script tracker to given number of seconds.
  
  ## Refresh(long seconds)
  Grant, if any, acquired number of point based on given number of seconds.

