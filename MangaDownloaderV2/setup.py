"""
Fichier d'installation de notre script salut.py."""
"""
import cfscrape
import sys

from cx_Freeze import setup, Executable

# On appelle la fonction setup
setup(
    name = "salut",
    version = "0.1",
    description = "Ce programme vous dit bonjour",
    executables = [Executable("test.py")],
)

# Let's start with some default (for me) imports...
"""
import sys
from cx_Freeze import setup, Executable

options = {
    'build_exe': {
        'includes': [
            'cfscrape',
            'urllib3'
        ],
        'path': sys.path + ['modules']
    }
}

executables = [
    Executable('test.py')
]

setup(name='advanced_cx_Freeze_sample',
      version='0.1',
      description='Advanced sample cx_Freeze script',
      options=options,
      executables=executables
      )