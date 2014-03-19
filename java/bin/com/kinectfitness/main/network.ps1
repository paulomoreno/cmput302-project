#create hosted network
netsh wlan set hostednetwork mode=allow ssid=kinectfitness key=kinect14
#start network
netsh wlan start hostednetwork