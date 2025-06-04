// This file globally aliases System.Windows.Forms to Modern.Forms for cross-platform migration.
// Add more aliases as needed for other namespaces/classes.
global using System.Windows.Forms;
global using System.Drawing;
#if !WINDOWS_OWN
//global using Application = System.Windows.Forms.Application2;
#endif
