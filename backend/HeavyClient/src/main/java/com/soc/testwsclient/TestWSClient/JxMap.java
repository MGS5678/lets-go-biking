package com.soc.testwsclient.TestWSClient;

import org.jxmapviewer.JXMapViewer;
import org.jxmapviewer.OSMTileFactoryInfo;
import org.jxmapviewer.viewer.DefaultTileFactory;
import org.jxmapviewer.viewer.GeoPosition;
import org.jxmapviewer.viewer.TileFactoryInfo;

import javax.swing.*;
import java.io.IOException;
import java.net.URL;

public class JxMap {
    public static void main(String[] args) {
        System.setProperty("http.agent", "MyJavaMapApp/1.0 (contact: you@example.com)");
        try {
            new URL("http://tile.openstreetmap.org/1/0/0.png").openStream();
        } catch (IOException e) {
            e.printStackTrace();
        }
        SwingUtilities.invokeLater(() -> {
            JXMapViewer mapViewer = new JXMapViewer();

            // Create a TileFactoryInfo for OpenStreetMap
            TileFactoryInfo info = new TileFactoryInfo(
                    0, 20, 20,
                    256, true, true,
                    "https://stamen-tiles.a.ssl.fastly.net/toner/",
                    "x", "y", "z") {

                @Override
                public String getTileUrl(int x, int y, int zoom) {
                    int invZoom = this.getTotalMapZoom() - zoom;
                    return String.format("https://stamen-tiles.a.ssl.fastly.net/toner/%d/%d/%d.png", invZoom, x, y);
                }
            };
            DefaultTileFactory tileFactory = new DefaultTileFactory(info);
            mapViewer.setTileFactory(tileFactory);

            // Use 8 threads in parallel to load the tiles
            tileFactory.setThreadPoolSize(8);

            // Set the focus
            GeoPosition frankfurt = new GeoPosition(50.11, 8.68);

            mapViewer.setZoom(7);
            mapViewer.setAddressLocation(frankfurt);

            // Display the viewer in a JFrame
            JFrame frame = new JFrame("JXMapviewer2 Example 1");
            frame.getContentPane().add(mapViewer);
            frame.setSize(800, 600);
            frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.setVisible(true);
        });
    }
}
