clear all;
close all;
clc;

img = imread("ud1.jpg");
feat = lbp(img);
figure, bar(feat);

img = imread("ud2.jpg");
feat = lbp(img);
figure, bar(feat);

img = imread("tower.jpg");
feat = lbp(img);
figure, bar(feat);
