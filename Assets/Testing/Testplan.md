# Testplan
## Vorraussetzungen
* Debug.Logs auskommentiert
* Testpositionen eingerichtet
* Testpinkte bestimmt
* Sinnvolle Kennzahlen festgelegt
* ANalyse Tool ausgesucht
## 1) Baseline: VR User ohne DoF + Fov
* Wieviel Performance braucht das Level ohne Depth of Field und Foveated Rendering?
* Dient als Baseline 
=> "Normalzustand"
## 2) Fall 1: VR User mit DoF 
* Wieviel Performance kostet mein Shader?
* Dient als zweite "Baseline"
* Unterschied zwischen DoF => DoF + Fov => DoF + Fov (optimiert)
## Fall 2: VR User mit DoF + Fov
* Wieviel bringt Foveated Rendering als Optimierung? 
* Erzeugt es Artefakte? 
* Wie stark nimmt der User die Artefakte wahr?
## Optimierung: VR User mit DoF + Fov (optimiert)
* Wieviel Performance - Gewinn bringt die Einschränkung auf den In-Focus Bereich?
* Erzeugt es Artefakte?
* Wie stark nimmt der User die Artefakte wahr?
