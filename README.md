# Collections-Legacy-ASP
Example files from an older ASP application.

This app is used by finance teams to manage accounts past due. In addition to security fixes, I was tasked with building out a locking mechanism for administrators, so they could lock the agents out of the app and prevent them from working tickets. The lock has three different tiers - A.) 'Global Lock', which is a simple on-off toggle that will lock/unlock every report on the app at the same time, B.) 'Locking Schedule', where administrators can enter a Day/Timespan (e.g. Tuesday from 10:00AM - 2:00PM) in which the reports will automatically lock, and C.) 'Report Locks', where each of the 70+ reports can be locked/unlocked individually.
