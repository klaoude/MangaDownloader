#!/usr/bin/env python
import cfscrape
import sys

scraper = cfscrape.create_scraper() # returns a requests.Session object
fd = open("html.txt", "w")
s = scraper.get(sys.argv[1]).content
fd.write(str(s))
fd.close()  
print(s)