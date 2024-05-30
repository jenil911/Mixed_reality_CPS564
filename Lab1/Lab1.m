disp('Welcome to Visual Computing and Mixed Reality Class!');

a = [1 2 3 4 5 6];
disp(a);

b = [6 5 4 3 2 1];
c = 5:10;

a = [1 2 3 4 5 6];
count = 0;
for i = 1:6
    count = count + a(i);
end
disp(count);
disp(sum(a));

size_a = size(a);
sum_a = sum(a);
mean_a = mean(a);
a_squared = a.^2;
a_plus_b = a + b;
a_minus_c = a - c;

BB = zeros(5,4);
disp(BB);
CC = ones(5,4);
disp(CC);

D = [1 2 3; 4 5 6; 7 8 9];
E = D';
disp(D);
disp(E);

img = imread('NASA_image.jpg');
disp(size(img));
figure, imshow(img);

img2 = imresize(img, [500 500]);
figure,imshow(img2);

img3 = imrotate( img , 90);
figure, imshow(img3);

