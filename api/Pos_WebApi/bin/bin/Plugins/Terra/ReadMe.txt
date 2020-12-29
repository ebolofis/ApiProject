Plugins για geolocation Με χρήση Terra Maps. 

-- ΓΕΝΙΚΑ --
 *  geolocation κατά τη λήψη παραγγελιών από τρίτα συστήματα delivery (πχ e-food), 
 *  προμήθεια των clients (pos, agent) με το απαραίτητο configuration για την εμφάνιση χαρτών, αναζήτηση διεθύνσεων και geolocation.



-- ΕΓΚΑΤΑΣΤΑΣΗ -- 
 Στο /bin/Plugins/ του WebAPI δημιουργούμε ένα καινούργιο φάκελο δίνουμε ένα όνομα και αντιγράφουμε το τρέχον κατάλογο εκεί.
	

-- CONFIGURATION -- 
Αρχείο settings.json περιέχει όλες τις απαιτούμενες μεταβλητές για τη διαχείριση
 

 *	IMapGeocode_Language:
		Την επιλεγμένη γλώσσα
		πχ el,en ...

*	IMapGeocode_RequestUrl
		Το Απαιτούμενο URL για να κάνουμε κλήση στο api της Terra ώστε να πραγματοποιηθεί το geolocation 
		πχ https://locators.terra.gr/TerraLocatorEverest/findAddressCandidatesEverest
		Το τροποποιούμε μόνο σε ενδεχόμενες αλλαγές της Terra

*	IMapGeocode_RemoveRegions:
		Λέξεις που θέλουν να παραλείπονται όταν πραγματοποιούμε μια νεα αναζήτηση
		πχ Αθήνα,Αθηνα,ΑΘΗΝΑ ...

*	IMapGeocode_ApiJs:
		Tο Απαιτούμενο URL για να φορτωθούν οι χάρτες της Terra
		πχ https://js.arcgis.com/4.12/
		Το τροποποιούμε μόνο σε ενδεχόμενες αλλαγές της Terra

*	IMapGeocode_UrlLatLng:
		Tο Απαιτούμενο URL για να κάνουμε κλήση στο api της Terra και να πάρουμε διεύθυνση με βάση τις συντεταγμένες
		πχ http://locators.terra.gr/terralocatoreverest/reverseGeocodeEverest
		Το τροποποιούμε μόνο σε ενδεχόμενες αλλαγές της Terra

*	IMapGeocode_MapInit:
		Tο Απαιτούμενο URL για να φορτώσουμε τους χάρτες με βάση τα στοιχεία του Javascript Api (ArcGis)
		πχ http://tiles.arcgis.com/tiles/mrOMWpWGs9ExyHNf/arcgis/rest/services/gr16Q4WMGr/VectorTileServer/resources/styles/root.json
		Το τροποποιούμε μόνο σε ενδεχόμενες αλλαγές του του Javascript Api (ArcGis)

*	IMapGeocode_MapCss:
		Tο Απαιτούμενο URL για να φορτώσουμε τα απαιτούμενα css για τους χάρτες με βάση τα στοιχεία του Javascript Api (ArcGis)
		πχ https://js.arcgis.com/4.12/esri/themes/light/main.css
		Το τροποποιούμε μόνο σε ενδεχόμενες αλλαγές του του Javascript Api (ArcGis)