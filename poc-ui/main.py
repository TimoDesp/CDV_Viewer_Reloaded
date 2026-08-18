from PySide6.QtWidgets import QApplication, QMainWindow, QDockWidget, QWidget, QTextEdit
from PySide6.QtGui import QPainter, QLinearGradient, QColor
from PySide6.QtCore import Qt, QRectF
import sys

class GradientMainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("CDV Viewer (PoC)")
        self.setMinimumSize(800, 600)

        # central widget placeholder (equivalent to DockContainer)
        central = QTextEdit("Zone principale (viewer)\n(ici on mettra QGraphicsView)")
        central.setReadOnly(True)
        self.setCentralWidget(central)

        # example docks equivalent to the original controls
        left_dock = QDockWidget("ListeLignes", self)
        left_dock.setWidget(QTextEdit("ListeLignes"))
        left_dock.setAllowedAreas(Qt.LeftDockWidgetArea | Qt.RightDockWidgetArea)
        self.addDockWidget(Qt.LeftDockWidgetArea, left_dock)

        right_dock = QDockWidget("HelpControl", self)
        right_dock.setWidget(QTextEdit("HelpControl"))
        self.addDockWidget(Qt.RightDockWidgetArea, right_dock)

        top_dock = QDockWidget("LiveControl", self)
        top_dock.setWidget(QTextEdit("LiveControl"))
        self.addDockWidget(Qt.TopDockWidgetArea, top_dock)

        bottom_dock = QDockWidget("ModeleViewer", self)
        bottom_dock.setWidget(QTextEdit("ModeleViewer"))
        self.addDockWidget(Qt.BottomDockWidgetArea, bottom_dock)

    def paintEvent(self, event):
        # draw background gradient (equivalent OnPaintBackground)
        painter = QPainter(self)
        rect = self.rect()
        grad = QLinearGradient(rect.topLeft(), rect.bottomLeft())
        grad.setColorAt(0.0, QColor(240, 237, 232))  # top color
        grad.setColorAt(1.0, QColor(220, 215, 210))  # bottom color
        painter.fillRect(QRectF(rect), grad)
        super().paintEvent(event)

if __name__ == "__main__":
    app = QApplication(sys.argv)
    win = GradientMainWindow()
    win.show()
    sys.exit(app.exec())
