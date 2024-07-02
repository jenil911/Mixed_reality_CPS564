close all;
clear all;
clc;
folder = './Datasets/';
files = dir(fullfile(folder, '*.jpg'));
feats = zeros(length(files), 256);
for i = 1:length(files)
    disp(i);
    filename = files(i,1).name;
    img = imread([folder filename]);
    img = imresize(img,[320,480]);
    feat = lbp(img);
    feats(i,:) = feat;
end

query = imread('query3.jpg');
query = imresize(query,[320,480]);
feat = lbp(query);

dists = distChiSq(feat,feats);

[val, idx] = min(dists);

img_result = imread([folder files(idx,1).name]);
img_result = imresize(img_result,[320,480]);
subplot(1,2,1); imshow(query); title('Query');
subplot(1,2,2); imshow(img_result); title('Result');



