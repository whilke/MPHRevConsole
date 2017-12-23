# MPHRevConsole
Display past 24h revenue in your currency of choice for MultiPoolHub mining

# Tip jar

**BTC:** 1tfZDwHYQiKVfXNJzY1339uTquiknmsq7  
**BCH:** 1657vgUNN2f7PzY1s9jcVrAY5NpCT2MyLt  
**ETH:** 0xf89d8344D615F3F918c47Bf45B327B3b3816baaF  
**LTC:** LSCDvpuT8xZTnAZtgrLtTmDvi9ebV4Brb2  

# Usage
MPHRevConsole.exe <autoconvert_coinname> <currency> <MPH_API>

- *< autoconvert_coinname >* - name of the coin that you are auto converting **TO:**
- * i.e litecoin, auroracoin-qubit, bitcoin, myriadcoin-skein, etc
- *< currency >* - Fiat or coin symbol to show earnings in:
- * i.e USD, BTC, LTC, etc
- *< MPH_API >* - Your MultiPoolHub API key

*example:* MPHRevConsole.exe litecoin USD <APIKey>

# How earnings are caluclated
Your last 24h earnings are calculated by taking the amount you've earned in your auto-convert-to coin for the past 24h and then adding in your current coins waiting to be exchanged (confirmed and unconfirmed).

Because of how MPH handles exchanges, it's possible that some of your auto converted coins will disapear from your report before it shows up on your converted report. However, spread across a 24h period this issue is typically reduced.

The earnings price can be volitile if the current coin market is also volitile, or if exchanges are taking longer then normal.

# New miners
The earnings data will only become stable once you have a solid consecutive 24-30 hours of mine time in. If you only mine for a part of the day, that's fine. As long as it's consistant.

Until that point you can use the earnings table and caculate a rough estimate by comparing the earnings increase after 6-8 hours.
